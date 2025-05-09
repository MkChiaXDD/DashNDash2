using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [Header("Dash Distance Settings")]
    [Tooltip("Percent of screen width to use for dash distance (0–1)")]
    [Range(0f, 1f)]
    public float dashPercent = 0.1f;

    /// <summary>
    /// Returns a Vector2 where both components equal the world-space dash distance,
    /// calculated as dashPercent of the **screen width**.
    /// </summary>
    public Vector2 GetDashDistance()
    {
        // Convert dashPercent of screen width into world units
        float worldW = ScreenSizeHelper.PercentWidthToWorld(dashPercent);
        // Always use width-based distance
        return new Vector2(worldW, worldW);
    }
}
