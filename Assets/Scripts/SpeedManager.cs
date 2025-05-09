using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float dashPercent = 0.1f; // % of the smaller screen dimension

    public Vector2 GetDashDistance()
    {
        // world units for that percent of width…
        float worldW = ScreenSizeHelper.PercentWidthToWorld(dashPercent);
        // …and for that percent of height
        float worldH = ScreenSizeHelper.PercentHeightToWorld(dashPercent);
        // pick the smaller so it never overshoots on the short side
        float worldDist = Mathf.Min(worldW, worldH);
        return new Vector2(worldDist, worldDist);
    }
}
