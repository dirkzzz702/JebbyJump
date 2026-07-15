using UnityEditor;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // Approved ProjectSettings writer (art batch 001 integration): assigns the
    // generated launcher icons to the Android adaptive/round/legacy slots and
    // the cross-platform default icon. Idempotent - re-running with the same
    // textures makes no change. Uses reflection for the Android icon-kind API
    // (same pattern as the P26 release tooling: custom asmdefs cannot reference
    // the Android extension assembly directly, and reflection also keeps this
    // compiling when Android support is absent).
    public static class AssignLauncherIcons
    {
        private const string IconFolder = "Assets/_JebbyJump/Art/Icons/";
        private const string Foreground = IconFolder + "ic_launcher_foreground_432.png";
        private const string Background = IconFolder + "ic_launcher_background_432.png";
        private const string Store512 = "StoreAssets/Icons/jebby_jump_icon_512.png";

        [MenuItem("Jebby Jump/Release/Assign Launcher Icons")]
        public static void Run()
        {
            var fg = AssetDatabase.LoadAssetAtPath<Texture2D>(Foreground);
            var bg = AssetDatabase.LoadAssetAtPath<Texture2D>(Background);
            if (fg == null || bg == null)
            {
                Debug.LogError("[AssignLauncherIcons] foreground/background texture missing under " + IconFolder);
                return;
            }

            int changes = 0;

            // Cross-platform default icon (also feeds the Android legacy slot
            // when kind-specific icons are absent). StoreAssets/ is outside
            // Assets/, so the 512 cannot be a Unity texture - use the adaptive
            // foreground as the default-icon texture instead (Unity scales it).
            var defaults = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
            if (defaults.Length == 0 || defaults[0] != fg)
            {
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new[] { fg });
                changes++;
            }

            changes += AssignAndroidKind("Adaptive", new[] { bg, fg });
            changes += AssignAndroidKind("Round", new[] { fg });
            changes += AssignAndroidKind("Legacy", new[] { fg });

            AssetDatabase.SaveAssets();
            Debug.Log("[AssignLauncherIcons] done; " + changes + " change group(s). "
                + "Monochrome layer: not exposed by this Unity version's icon API - "
                + "tracked in ProductionArtAudit/integration (Gradle template note).");
        }

        // Assigns textures to every icon size of the given Android kind.
        // Returns 1 when anything changed. Layer order for Adaptive: Unity's
        // PlatformIcon layer 0 = background, layer 1 = foreground.
        private static int AssignAndroidKind(string kindName, Texture2D[] layers)
        {
            var kindType = FindType("UnityEditor.Android.AndroidPlatformIconKind");
            if (kindType == null)
            {
                Debug.LogWarning("[AssignLauncherIcons] Android icon API unavailable ("
                    + kindName + " skipped) - is Android build support installed?");
                return 0;
            }
            var kindField = kindType.GetField(kindName,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (kindField == null) return 0;
            var kind = kindField.GetValue(null);

            // Unity 6 exposes Get/SetPlatformIcons with a NamedBuildTarget first
            // parameter (older versions used BuildTargetGroup) - match either.
            System.Reflection.MethodInfo getIcons = null, setIcons = null;
            object targetArg = null;
            foreach (var m in typeof(PlayerSettings).GetMethods(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                var ps = m.GetParameters();
                if (m.Name == "GetPlatformIcons" && ps.Length == 2
                    && ps[1].ParameterType.IsInstanceOfType(kind))
                {
                    getIcons = m;
                    targetArg = MakeTargetArg(ps[0].ParameterType);
                }
                if (m.Name == "SetPlatformIcons" && ps.Length == 3
                    && ps[1].ParameterType.IsInstanceOfType(kind))
                    setIcons = m;
            }
            if (getIcons == null || setIcons == null || targetArg == null)
            {
                Debug.LogWarning("[AssignLauncherIcons] PlatformIcons API not found; " + kindName + " skipped");
                return 0;
            }

            var icons = (object[])getIcons.Invoke(null, new[] { targetArg, kind });
            if (icons == null || icons.Length == 0) return 0;

            var iconType = icons[0].GetType();
            var setTex = iconType.GetMethod("SetTextures", new[] { typeof(Texture2D[]) });
            var getTex = iconType.GetMethod("GetTextures", System.Type.EmptyTypes);
            var layerCount = iconType.GetProperty("LayerCount");

            bool changed = false;
            foreach (var icon in icons)
            {
                int count = layerCount != null ? (int)layerCount.GetValue(icon) : layers.Length;
                var want = new Texture2D[count];
                for (int i = 0; i < count; i++)
                    want[i] = layers[Mathf.Min(i, layers.Length - 1)];
                var have = getTex != null ? (Texture2D[])getTex.Invoke(icon, null) : null;
                bool same = have != null && have.Length == count;
                if (same)
                    for (int i = 0; i < count; i++)
                        if (have[i] != want[i]) { same = false; break; }
                if (same) continue;
                setTex.Invoke(icon, new object[] { want });
                changed = true;
            }
            if (!changed) return 0;

            var typedArray = System.Array.CreateInstance(iconType, icons.Length);
            icons.CopyTo(typedArray, 0);
            setIcons.Invoke(null, new[] { targetArg, kind, typedArray });
            Debug.Log("[AssignLauncherIcons] " + kindName + ": assigned " + icons.Length + " icon size(s)");
            return 1;
        }

        // Builds the first argument for Get/SetPlatformIcons: either the
        // BuildTargetGroup enum or a NamedBuildTarget struct (Unity 6).
        private static object MakeTargetArg(System.Type paramType)
        {
            if (paramType == typeof(BuildTargetGroup))
                return BuildTargetGroup.Android;
            var android = paramType.GetProperty("Android",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (android != null) return android.GetValue(null);
            var fromGroup = paramType.GetMethod("FromBuildTargetGroup",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            return fromGroup != null
                ? fromGroup.Invoke(null, new object[] { BuildTargetGroup.Android })
                : null;
        }

        private static System.Type FindType(string fullName)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(fullName);
                if (t != null) return t;
            }
            return null;
        }
    }
}
