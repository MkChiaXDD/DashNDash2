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

        float hp = Mathf.Clamp01(heightPercent);
        float screenH = ScreenSizeHelper.ScreenWorldHeight;
        float targetH = screenH * hp;

        float objH = GetUnscaledObjectHeight(rend);
        if (objH <= 0f)
        {
            Debug.LogWarning($"[{t.name}] object height ≤ 0!");
            return;
        }

        float factor = targetH / objH;
        t.localScale = Vector3.one * factor;
    }

    /// <summary>
    /// Uniformly scales this transform so its renderer's width becomes
    /// widthPercent (0–1) of the screen-world width.
    /// </summary>
    public static void ScaleToScreenWidthPercent(this Transform t, float widthPercent)
    {
        var rend = t.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning($"[{t.name}] no Renderer found.");
            return;
        }

        float wp = Mathf.Clamp01(widthPercent);
        float screenW = ScreenSizeHelper.ScreenWorldWidth;
        float targetW = screenW * wp;

        float objW = GetUnscaledObjectWidth(rend);
        if (objW <= 0f)
        {
            Debug.LogWarning($"[{t.name}] object width ≤ 0!");
            return;
        }

        float factor = targetW / objW;
        t.localScale = Vector3.one * factor;
    }

    /// <summary>
    /// Uniformly scales this transform so it fits within widthPercent and heightPercent,
    /// taking the smaller scale factor to preserve aspect.
    /// </summary>
    public static void ScaleToScreenPercent(this Transform t, float widthPercent, float heightPercent)
    {
        // clamp
        float wp = Mathf.Clamp01(widthPercent);
        float hp = Mathf.Clamp01(heightPercent);

        float screenW = ScreenSizeHelper.ScreenWorldWidth;
        float screenH = ScreenSizeHelper.ScreenWorldHeight;

        float targetW = screenW * wp;
        float targetH = screenH * hp;

        var rend = t.GetComponent<Renderer>();
        if (rend == null)
        {
            Debug.LogWarning($"[{t.name}] no Renderer found.");
            return;
        }

        float objW = GetUnscaledObjectWidth(rend);
        float objH = GetUnscaledObjectHeight(rend);
        if (objW <= 0f || objH <= 0f)
        {
            Debug.LogWarning($"[{t.name}] object size ≤ 0!");
            return;
        }

        float factorW = targetW / objW;
        float factorH = targetH / objH;
        float factor = Mathf.Min(factorW, factorH);
        t.localScale = Vector3.one * factor;
    }

    private static float GetUnscaledObjectHeight(Renderer rend)
    {
        if (rend is SpriteRenderer sr && sr.sprite != null)
            return sr.sprite.bounds.size.y;

        var mf = rend.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return mf.sharedMesh.bounds.size.y;

        return rend.localBounds.size.y;
    }

    private static float GetUnscaledObjectWidth(Renderer rend)
    {
        if (rend is SpriteRenderer sr && sr.sprite != null)
            return sr.sprite.bounds.size.x;

        var mf = rend.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return mf.sharedMesh.bounds.size.x;

        return rend.localBounds.size.x;
    }
}