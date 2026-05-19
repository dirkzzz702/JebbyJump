using JebbyJump.World;
using UnityEditor;
using UnityEngine;

public static class DiagnoseBounds
{
    public static void Run()
    {
        var bg = GameObject.Find("Background");
        if (bg == null) { Debug.LogError("[Diag] Background not found."); return; }

        var sr = bg.GetComponent<SpriteRenderer>();
        if (sr == null) { Debug.LogError("[Diag] No SpriteRenderer."); return; }

        // Compute expected world bounds from sprite + scale (editor-safe)
        float sprW  = sr.sprite != null ? sr.sprite.bounds.size.x : 0f;
        float sprH  = sr.sprite != null ? sr.sprite.bounds.size.y : 0f;
        float sprPX = sr.sprite != null ? sr.sprite.pivot.x / sr.sprite.rect.width : 0.5f;
        float sprPY = sr.sprite != null ? sr.sprite.pivot.y / sr.sprite.rect.height : 0.5f;

        float worldW = sprW * bg.transform.localScale.x;
        float worldH = sprH * bg.transform.localScale.y;
        float posX   = bg.transform.position.x;
        float posY   = bg.transform.position.y;

        float leftEdge  = posX - sprPX * worldW;
        float rightEdge = posX + (1f - sprPX) * worldW;
        float botEdge   = posY - sprPY * worldH;
        float topEdge   = posY + (1f - sprPY) * worldH;

        Debug.Log($"[Diag] BG pos=({posX:F2},{posY:F2})  scale=({bg.transform.localScale.x:F2},{bg.transform.localScale.y:F2})");
        Debug.Log($"[Diag] Sprite natural: {sprW:F2}w x {sprH:F2}h  pivot=({sprPX:F3},{sprPY:F3})");
        Debug.Log($"[Diag] World size: {worldW:F2}w x {worldH:F2}h");
        Debug.Log($"[Diag] Computed bounds X: [{leftEdge:F2}, {rightEdge:F2}]  center: {(leftEdge+rightEdge)/2f:F2}");
        Debug.Log($"[Diag] Computed bounds Y: [{botEdge:F2}, {topEdge:F2}]  center: {(botEdge+topEdge)/2f:F2}");

        // Camera info
        var cam = Camera.main;
        if (cam != null)
        {
            float hw = cam.orthographicSize * cam.aspect;
            Debug.Log($"[Diag] Camera ortho={cam.orthographicSize}  half-width={hw:F2}  pos=({cam.transform.position.x:F2},{cam.transform.position.y:F2})");
            Debug.Log($"[Diag] Camera sees X: [{cam.transform.position.x - hw:F2}, {cam.transform.position.x + hw:F2}]");
        }

        // CameraXConfiner
        var confiner = cam != null ? cam.GetComponent<CameraXConfiner>() : null;
        if (confiner != null)
        {
            var so = new SerializedObject(confiner);
            var bgProp = so.FindProperty("_background");
            Debug.Log($"[Diag] CameraXConfiner._background assigned: {bgProp.objectReferenceValue != null}");
        }

        // --- Compute required BG position to center at x=0 ---
        float requiredPosX = sprPX * worldW;  // so that leftEdge = 0 - sprPX*worldW = 0 when posX=sprPX*worldW
        // Actually: leftEdge = posX - sprPX*worldW = 0 → posX = sprPX*worldW
        // But we want leftEdge = -worldW/2, so posX = -worldW/2 + sprPX*worldW
        float centeredPosX = -worldW / 2f + sprPX * worldW;
        Debug.Log($"[Diag] To center BG at world x=0: background.transform.position.x should be {centeredPosX:F2}");
    }
}
