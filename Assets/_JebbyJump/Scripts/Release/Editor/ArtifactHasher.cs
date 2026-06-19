using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace JebbyJump.Release
{
    // Hashes complete distributables (fix #10). Android: the single .aab.
    // Windows smoke: a sorted per-file manifest with a SHA-256 per file (the whole
    // build directory, not just the .exe). Paths are reported relative to a root.
    public static class ArtifactHasher
    {
        [Serializable]
        public struct FileHash
        {
            public string RelativePath;
            public string Sha256;
            public long Bytes;
        }

        public static string Sha256File(string path)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
            {
                byte[] hash = sha.ComputeHash(stream);
                return ToHex(hash);
            }
        }

        // Sorted (ordinal) manifest of every file under dir, paths relative to
        // relativeRoot so no local machine paths leak into the report.
        public static FileHash[] Sha256Directory(string dir, string relativeRoot)
        {
            var files = new List<string>(
                Directory.GetFiles(dir, "*", SearchOption.AllDirectories));
            files.Sort(StringComparer.Ordinal);
            var result = new FileHash[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                var info = new FileInfo(files[i]);
                result[i] = new FileHash
                {
                    RelativePath = Relative(relativeRoot, files[i]),
                    Sha256 = Sha256File(files[i]),
                    Bytes = info.Length,
                };
            }
            return result;
        }

        public static string Relative(string root, string full)
        {
            string r = full.Replace('\\', '/');
            string b = (root ?? "").Replace('\\', '/');
            if (!string.IsNullOrEmpty(b) && r.StartsWith(b, StringComparison.OrdinalIgnoreCase))
            {
                r = r.Substring(b.Length);
                if (r.StartsWith("/")) r = r.Substring(1);
            }
            return r;
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new System.Text.StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
