// File: ScreenScaleExtensions.cs
using UnityEngine;

public static class GameObjectScaler
{
    /// <summary>
    /// Uniformly scales this transform so its renderer's height becomes
    /// heightPercent (0–1) of the screen-world height.
    /// </summary>
    public static void ScaleToScreenHeightPercent(this Transform t, float heightPercent)
    {
        var rend = t.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning($"[{t.name}] no Renderer found.");
            return;
        }

        // clamp percent
        float pct = Mathf.Clamp01(heightPercent);
        // world-space screen height
        float screenH = ScreenSizeHelper.ScreenWorldHeight;
        // desired world-space height
        float targetH = screenH * pct;

        // figure out your object's unscaled height
        float objHeight = GetUnscaledObjectHeight(rend);
        if (objHeight <= 0f)
        {
            Debug.LogWarning($"[{t.name}] object height ≤ 0!");
            return;
        }

        // compute and apply uniform scale factor
        float factor = targetH / objHeight;
        t.localScale = Vector3.one * factor;

        Debug.Log(
            $"[{t.name}] ScaleToScreenHeightPercent: " +
            $"screenH={screenH:F2}, pct={pct:F3}, " +
            $"objH={objHeight:F3}, factor={factor:F3}");
    }

    // Helper: prefer Sprite bounds, then Mesh bounds, then Renderer.localBounds
    private static float GetUnscaledObjectHeight(Renderer rend)
    {
        // SpriteRenderer? use sprite.bounds (in local units)
        if (rend is SpriteRenderer sr && sr.sprite != null)
            return sr.sprite.bounds.size.y;

        // MeshFilter? use sharedMesh.bounds
        var mf = rend.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return mf.sharedMesh.bounds.size.y;

        // fallback to localBounds (unscaled)
        return rend.localBounds.size.y;
    }
}
