using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace JebbyJump.Release
{
    // Read-only inspection of a built Android artifact using the real toolchain
    // (P26-Sign corrections #3/#4/#5). Locates apksigner / aapt2 / zipalign in the
    // Android SDK build-tools and jarsigner in the JDK. Every method degrades to a
    // HONEST "Skipped" (never a false PASS) when a tool or artifact is unavailable.
    public static class AndroidArtifactInspector
    {
        [Serializable]
        public struct SignatureResult
        {
            public string Status;            // "Verified" | "Failed" | "Skipped"
            public string Tool;              // "apksigner" | "jarsigner" | ""
            public string CertSha256;        // public signer cert fingerprint (NOT secret)
            public string Detail;
        }

        [Serializable]
        public struct PageSizeResult
        {
            public string Status;            // "Aligned16k" | "NotAligned16k" | "Skipped"
            public string Detail;
        }

        [Serializable]
        public struct TargetSdkResult
        {
            public int ResolvedTargetSdk;    // 0 = unknown/unread
            public string Status;            // "Read" | "Skipped"
            public string Detail;
        }

        // ---- signature verification (correction #3) ----

        public static SignatureResult VerifyApk(string apkPath)
        {
            if (!File.Exists(apkPath))
                return new SignatureResult { Status = "Skipped", Detail = "apk missing" };
            string apksigner = FindBuildTool("apksigner.bat") ?? FindBuildTool("apksigner");
            if (apksigner == null)
                return new SignatureResult { Status = "Skipped", Tool = "apksigner", Detail = "apksigner not found" };
            var (code, outp, err) = Run(apksigner, $"verify --print-certs \"{apkPath}\"");
            string fp = Match(outp, @"SHA-256 digest:\s*([0-9a-fA-F]{64})");
            bool verified = code == 0;
            return new SignatureResult
            {
                Status = verified ? "Verified" : "Failed",
                Tool = "apksigner",
                CertSha256 = fp ?? "",
                Detail = verified ? "apksigner verify exit 0" : Trim(err + " " + outp),
            };
        }

        public static SignatureResult VerifyAab(string aabPath)
        {
            if (!File.Exists(aabPath))
                return new SignatureResult { Status = "Skipped", Detail = "aab missing" };
            // An AAB is a jar-signed zip (Play re-signs on distribution). jarsigner -verify
            // confirms the upload/debug signature integrity of the bundle.
            string jarsigner = FindJdkTool("jarsigner");
            if (jarsigner == null)
                return new SignatureResult { Status = "Skipped", Tool = "jarsigner", Detail = "jarsigner not found" };
            var (code, outp, err) = Run(jarsigner, $"-verify \"{aabPath}\"");
            bool verified = code == 0 && outp.IndexOf("verified", StringComparison.OrdinalIgnoreCase) >= 0;
            return new SignatureResult
            {
                Status = verified ? "Verified" : "Failed",
                Tool = "jarsigner",
                CertSha256 = "", // jarsigner -verify does not emit a SHA-256 cert digest
                Detail = verified ? Trim(outp) : Trim(err + " " + outp),
            };
        }

        // ---- 16 KB page-size alignment (correction #5) ----

        public static PageSizeResult CheckApk16kAlignment(string apkPath)
        {
            if (!File.Exists(apkPath))
                return new PageSizeResult { Status = "Skipped", Detail = "apk missing" };
            string zipalign = FindBuildTool("zipalign.exe") ?? FindBuildTool("zipalign");
            if (zipalign == null)
                return new PageSizeResult { Status = "Skipped", Detail = "zipalign not found" };
            // -c check only, -P 16 = verify .so files are aligned to 16 KiB pages.
            var (code, outp, err) = Run(zipalign, $"-c -P 16 4 \"{apkPath}\"");
            return new PageSizeResult
            {
                Status = code == 0 ? "Aligned16k" : "NotAligned16k",
                Detail = code == 0 ? "zipalign -c -P 16 exit 0" : Trim(err + " " + outp),
            };
        }

        // ---- resolved artifact target SDK (correction #4) ----

        public static TargetSdkResult ReadApkTargetSdk(string apkPath)
        {
            if (!File.Exists(apkPath))
                return new TargetSdkResult { Status = "Skipped", Detail = "apk missing" };
            string aapt2 = FindBuildTool("aapt2.exe") ?? FindBuildTool("aapt2");
            if (aapt2 == null)
                return new TargetSdkResult { Status = "Skipped", Detail = "aapt2 not found" };
            var (code, outp, err) = Run(aapt2, $"dump badging \"{apkPath}\"");
            string sdk = Match(outp, @"targetSdkVersion:'(\d+)'");
            if (code == 0 && sdk != null && int.TryParse(sdk, out int n))
                return new TargetSdkResult { ResolvedTargetSdk = n, Status = "Read", Detail = "" };
            return new TargetSdkResult { Status = "Skipped", Detail = Trim(err + " " + outp) };
        }

        // Resolves "Automatic" target SDK to the highest installed Android platform
        // (deterministic, no build needed) - the value an Automatic config would pick.
        public static int HighestInstalledPlatformApi()
        {
            string sdk = AndroidSdkRoot();
            if (string.IsNullOrEmpty(sdk)) return 0;
            string platforms = Path.Combine(sdk, "platforms");
            if (!Directory.Exists(platforms)) return 0;
            int best = 0;
            foreach (var dir in Directory.GetDirectories(platforms))
            {
                var m = Regex.Match(Path.GetFileName(dir), @"android-(\d+)");
                if (m.Success && int.TryParse(m.Groups[1].Value, out int api) && api > best) best = api;
            }
            return best;
        }

        // ---- tool location ----

        private static string FindBuildTool(string exe)
        {
            string sdk = AndroidSdkRoot();
            if (string.IsNullOrEmpty(sdk)) return null;
            string bt = Path.Combine(sdk, "build-tools");
            if (!Directory.Exists(bt)) return null;
            string best = null; int bestVer = -1;
            foreach (var dir in Directory.GetDirectories(bt))
            {
                string candidate = Path.Combine(dir, exe);
                if (!File.Exists(candidate)) continue;
                int v = VersionKey(Path.GetFileName(dir));
                if (v > bestVer) { bestVer = v; best = candidate; }
            }
            return best;
        }

        private static int VersionKey(string name)
        {
            var parts = name.Split('.');
            int key = 0;
            foreach (var p in parts)
            {
                int.TryParse(p, out int n);
                key = key * 1000 + Math.Min(n, 999);
            }
            return key;
        }

        private static string FindJdkTool(string tool)
        {
            // Prefer the JDK Unity is configured to use; fall back to PATH.
            string jdk = ReflectExternalToolPath("jdkRootPath");
            if (!string.IsNullOrEmpty(jdk))
            {
                string p = Path.Combine(jdk, "bin", tool + ".exe");
                if (File.Exists(p)) return p;
                p = Path.Combine(jdk, "bin", tool);
                if (File.Exists(p)) return p;
            }
            return OnPath(tool + ".exe") ?? OnPath(tool);
        }

        private static string AndroidSdkRoot()
        {
            string sdk = ReflectExternalToolPath("sdkRootPath");
            if (!string.IsNullOrEmpty(sdk) && Directory.Exists(sdk)) return sdk;
            string pref = UnityEditor.EditorPrefs.GetString("AndroidSdkRoot");
            if (!string.IsNullOrEmpty(pref) && Directory.Exists(pref)) return pref;
            return null;
        }

        private static string ReflectExternalToolPath(string prop)
        {
            try
            {
                var t = Type.GetType(
                    "UnityEditor.Android.AndroidExternalToolsSettings, UnityEditor.Android.Extensions");
                return t?.GetProperty(prop,
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    ?.GetValue(null) as string;
            }
            catch { return null; }
        }

        private static string OnPath(string exe)
        {
            string path = Environment.GetEnvironmentVariable("PATH") ?? "";
            foreach (var dir in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrEmpty(dir)) continue;
                try { string p = Path.Combine(dir, exe); if (File.Exists(p)) return p; }
                catch { }
            }
            return null;
        }

        // ---- process + parsing helpers ----

        private static (int code, string outp, string err) Run(string exe, string args)
        {
            try
            {
                var psi = new ProcessStartInfo(exe, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (var p = Process.Start(psi))
                {
                    string o = p.StandardOutput.ReadToEnd();
                    string e = p.StandardError.ReadToEnd();
                    p.WaitForExit(60000);
                    return (p.ExitCode, o, e);
                }
            }
            catch (Exception ex) { return (-1, "", ex.Message); }
        }

        private static string Match(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var m = Regex.Match(text, pattern);
            return m.Success ? m.Groups[1].Value : null;
        }

        private static string Trim(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            s = s.Trim();
            return s.Length > 240 ? s.Substring(0, 240) : s;
        }
    }
}
