using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decorative pre-storm ambience: drifts a set of (duplicated) clouds leftward across the screen
/// and wraps them around, each at its own speed/height. Purely visual — no cover, no holds.
/// The WeatherController calls StopDrift() when the storm begins, and each cloud then finishes
/// drifting off-screen before it retires.
/// </summary>
public class AmbientDrift : MonoBehaviour
{
    [Tooltip("The duplicated decorative cloud objects to drift. Not the 8 storm clouds.")]
    [SerializeField] private List<Transform> clouds = new List<Transform>();

    [Header("Drift")]
    [Tooltip("Clouds move -X (leftward, matching the storm). Speed randomized per pass.")]
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 2.5f;

    [Header("Camera / bounds")]
    [Tooltip("Camera used to compute screen edges + the top half. Leave empty to use Camera.main.")]
    [SerializeField] private Camera viewCamera;
    [Tooltip("Keep clouds this far below the top edge and above the horizontal midline.")]
    [SerializeField] private float verticalMargin = 0.5f;
    [Tooltip("How far PAST the screen edge a cloud must travel before wrapping/retiring, so its whole body clears view (~cloud half-width). Increase if bodies still show when they disappear.")]
    [SerializeField] private float edgePadding = 3.5f;
    [Tooltip("Extra random distance off the right edge on (re)entry, so clouds arrive staggered instead of in lockstep.")]
    [SerializeField] private float staggerSpread = 6f;

    private readonly Dictionary<Transform, float> speed = new Dictionary<Transform, float>();
    private bool running;
    private bool exiting; // storm started: keep drifting, but retire each cloud once it's off-screen

    private void Start()
    {
        ComputeHorizontal(out float leftExit, out float rightEnter);
        // Scatter across the view AND out to the staggered off-screen zone, so some are visible
        // immediately and others trickle in at different times.
        foreach (Transform c in clouds)
        {
            if (c == null) continue;
            PlaceCloud(c, Random.Range(leftExit, rightEnter + staggerSpread));
        }
        running = true;
    }

    private void Update()
    {
        if (!running) return;
        ComputeHorizontal(out float leftExit, out float rightEnter);
        foreach (Transform c in clouds)
        {
            if (c == null || !c.gameObject.activeSelf) continue;
            c.position += Vector3.left * (speed[c] * Time.deltaTime);
            if (c.position.x < leftExit)
            {
                if (exiting)
                    c.gameObject.SetActive(false);                              // fully off-screen after storm start -> retire
                else
                    PlaceCloud(c, rightEnter + Random.Range(0f, staggerSpread)); // wrap: reappear at a staggered offset
            }
        }
    }

    private void PlaceCloud(Transform c, float x)
    {
        ComputeTopHalf(out float minY, out float maxY);
        Vector3 p = c.position;
        p.x = x;
        p.y = Random.Range(minY, maxY);
        c.position = p;
        speed[c] = Random.Range(minSpeed, maxSpeed);
    }

    // Left edge a cloud must clear before wrapping/retiring, and the right edge it re-enters from —
    // both pushed a cloud-width beyond the actual view so nothing pops in/out on camera.
    private void ComputeHorizontal(out float leftExit, out float rightEnter)
    {
        Camera cam = viewCamera != null ? viewCamera : Camera.main;
        if (cam != null && cam.orthographic)
        {
            float halfWidth = cam.orthographicSize * cam.aspect;
            float camX = cam.transform.position.x;
            leftExit = camX - halfWidth - edgePadding;
            rightEnter = camX + halfWidth + edgePadding;
        }
        else
        {
            leftExit = -14f; // fallback if no orthographic camera is found
            rightEnter = 10f;
        }
    }

    // Top half of the orthographic view: from the horizontal midline (camera Y) up to the top edge.
    private void ComputeTopHalf(out float minY, out float maxY)
    {
        Camera cam = viewCamera != null ? viewCamera : Camera.main;
        if (cam != null && cam.orthographic)
        {
            float centerY = cam.transform.position.y;
            minY = centerY + verticalMargin;
            maxY = centerY + cam.orthographicSize - verticalMargin;
        }
        else
        {
            minY = 0.5f; // fallback if no orthographic camera is found
            maxY = 4.5f;
        }
    }

    /// <summary>Let the decorative clouds finish drifting off-screen, then retire each (called when the storm starts).</summary>
    public void StopDrift()
    {
        exiting = true; // stop wrapping; each cloud deactivates itself once it clears the left edge
    }
}
