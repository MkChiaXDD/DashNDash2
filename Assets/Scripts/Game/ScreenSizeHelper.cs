using UnityEngine;

public static class ScreenSizeHelper
{
    // World height based on ortho camera
    public static float ScreenWorldHeight => Camera.main.orthographicSize * 2f;

    // World width based on ortho camera aspect
    public static float ScreenWorldWidth => ScreenWorldHeight * Camera.main.aspect;

    // Convert 0–1 percent of screen height to world units
    public static float PercentHeightToWorld(float percent)
    {
        return Mathf.Clamp01(percent) * ScreenWorldHeight;
    }

    // Convert 0–1 percent of screen width to world units
    public static float PercentWidthToWorld(float percent)
    {
        return Mathf.Clamp01(percent) * ScreenWorldWidth;
    }
}
