using UnityEngine;
using UnityEditor;

public static class DiagnoseP1State
{
    public static void Run()
    {
        Debug.Log("=== P1 STATE DIAGNOSIS ===");

        // Background
        var bg = GameObject.Find("Background");
        if (bg != null)
        {
            var sr = bg.GetComponent<SpriteRenderer>();
            Debug.Log($"[BG] pos={bg.transform.position}  scale={bg.transform.localScale}  bounds={sr.bounds.min} -> {sr.bounds.max}");
        }

        // Floor
        var floor = GameObject.Find("Floor");
        if (floor != null)
        {
            var fc = floor.GetComponent<BoxCollider2D>();
            float topY = floor.transform.position.y + (fc != null ? fc.size.y * floor.transform.localScale.y / 2f + fc.offset.y * floor.transform.localScale.y : 0f);
            Debug.Log($"[Floor] pos={floor.transform.position}  scale={floor.transform.localScale}  colliderSize={fc?.size}  colliderOffset={fc?.offset}  topSurfaceY={topY:F3}");
        }

        // Jebby
        var jebby = GameObject.Find("Jebby");
        if (jebby != null)
        {
            var cap = jebby.GetComponentInChildren<CapsuleCollider2D>();
            Debug.Log($"[Jebby] pos={jebby.transform.position}  scale={jebby.transform.localScale}  capsuleSize={cap?.size}  capsuleOffset={cap?.offset}");
            // Compute capsule bottom in world
            if (cap != null)
            {
                float capBottomLocal = cap.offset.y - cap.size.y * 0.5f;
                float capBottomWorld = jebby.transform.position.y + capBottomLocal * jebby.transform.localScale.y;
                Debug.Log($"[Jebby] capsule bottom world y = {capBottomWorld:F3}");
            }
        }

        // Walls
        var wl = GameObject.Find("WallLeft");
        var wr = GameObject.Find("WallRight");
        if (wl != null) Debug.Log($"[WallLeft] pos={wl.transform.position}");
        if (wr != null) Debug.Log($"[WallRight] pos={wr.transform.position}");

        // Camera
        var cam = Camera.main;
        if (cam != null)
        {
            float hw = cam.orthographicSize * cam.aspect;
            Debug.Log($"[Cam] pos={cam.transform.position}  ortho={cam.orthographicSize}  halfW={hw:F2}");
        }

        // MobileControlsCanvas
        var mcc = GameObject.Find("MobileControlsCanvas");
        if (mcc != null)
        {
            Debug.Log($"[MobileControlsCanvas] active={mcc.activeSelf}  scale={mcc.transform.localScale}");
        }

        // LivesText / LivesIconContainer
        var lt = GameObject.Find("LivesText");
        if (lt != null) Debug.Log($"[LivesText] active={lt.activeSelf}");
        var lic = GameObject.Find("LivesIconContainer");
        if (lic != null) Debug.Log($"[LivesIconContainer] active={lic.activeSelf}");

        // TutorialHint
        var th = GameObject.Find("TutorialHint");
        if (th != null)
        {
            var rt = th.GetComponent<RectTransform>();
            Debug.Log($"[TutorialHint] active={th.activeSelf}  anchorMin={rt.anchorMin}  anchorMax={rt.anchorMax}  pivot={rt.pivot}  anchoredPos={rt.anchoredPosition}");
        }

        // GameOver / LevelComplete panels font assignment
        var gop = GameObject.Find("GameOverPanel");
        if (gop != null)
        {
            var texts = gop.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            foreach (var t in texts)
                Debug.Log($"[GameOverPanel] TMP '{t.name}' fontAsset={(t.font != null ? t.font.name : "NULL")}  text='{t.text}'");
        }
        var lcp = GameObject.Find("LevelCompletePanel");
        if (lcp != null)
        {
            var texts = lcp.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            foreach (var t in texts)
                Debug.Log($"[LevelCompletePanel] TMP '{t.name}' fontAsset={(t.font != null ? t.font.name : "NULL")}  text='{t.text}'");
        }
    }
}
