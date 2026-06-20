using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace JebbyJump.Tests
{
    // P24 correction #4: measure the COMPLETE scene load-to-readiness lifecycle
    // (async load + a frame for Awake/Start), not just the LoadScene call. Headless
    // wall-clock is a regression signal only, not a device FPS/load certification.
    public class SceneReadinessTests
    {
        [UnityTest]
        public IEnumerator MainMenu_LoadsToReadiness()
        {
            var sw = Stopwatch.StartNew();
            var op = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
            Assert.IsNotNull(op, "MainMenu must be in the build scene list (P23 contract)");
            while (!op.isDone) yield return null;
            yield return null; // allow Awake/Start to run -> readiness
            sw.Stop();

            var scene = SceneManager.GetActiveScene();
            Assert.AreEqual("MainMenu", scene.name);
            int roots = scene.GetRootGameObjects().Length;
            Assert.Greater(roots, 0, "MainMenu should have root objects after readiness");
            Debug.Log($"[P24][bench] MainMenu load-to-ready: {sw.Elapsed.TotalMilliseconds:F1} ms, roots={roots}");
        }
    }
}
