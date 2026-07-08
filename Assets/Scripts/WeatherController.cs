using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Decorative clouds drift until StartStorm(), which plays the "enter" timeline
/// (enter -> cover -> dim -> rain ramps up) and then holds that dark/rainy state.
/// EndStorm() plays the "exit" timeline (rain ramps down -> exit -> brighten) from there.
/// </summary>
[DisallowMultipleComponent]
public class WeatherController : MonoBehaviour
{
    private enum StormPhase { Idle, Active, Ended }

    [SerializeField] private AmbientDrift ambient;

    [Tooltip("The 8 storm clouds. Hidden during ambient; revealed when the storm plays.")]
    [SerializeField] private GameObject[] stormClouds;

    [Tooltip("Shared director that plays whichever storm timeline is currently assigned to it.")]
    [SerializeField] private PlayableDirector stormDirector;

    [Tooltip("Enter timeline: clouds enter -> cover -> light dims -> rain ramps up. Holds on the storm-active state.")]
    [SerializeField] private TimelineAsset stormEnterTimeline;

    [Tooltip("Exit timeline: rain ramps down -> clouds exit -> light brightens. Holds on the cleared state.")]
    [SerializeField] private TimelineAsset stormExitTimeline;

    [Tooltip("Rain particle system. Its emission RATE is animated by the timelines, but the system itself must be told to play — the controller does that when the storm starts.")]
    [SerializeField] private ParticleSystem rain;

    private StormPhase phase = StormPhase.Idle;

    private void Awake()
    {
        if (stormDirector != null)
        {
            stormDirector.playOnAwake = false;
            stormDirector.extrapolationMode = DirectorWrapMode.Hold; // hold the cleared/bright end state
            stormDirector.Stop();
        }
        if (rain != null)
            rain.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // no rain during ambient
        SetStormCloudsActive(false); // keep the real clouds hidden while the decorative ones drift
    }

    [ContextMenu("Start Storm")]
    public void StartStorm()
    {
        if (phase != StormPhase.Idle)
        {
            Debug.LogWarning($"[WeatherController] StartStorm ignored, phase is {phase}.", this);
            return;
        }
        if (stormDirector == null || stormEnterTimeline == null)
        {
            Debug.LogWarning("[WeatherController] StartStorm ignored, Storm Director or Storm Enter Timeline not assigned.", this);
            return;
        }
        phase = StormPhase.Active;

        if (ambient != null) ambient.StopDrift();
        SetStormCloudsActive(true);

        stormDirector.playableAsset = stormEnterTimeline;
        stormDirector.time = 0;
        stormDirector.Play();

        // The system must be playing for the timeline's emission-rate curve to actually emit;
        // it starts at rate 0 and the enter timeline ramps it up, then holds.
        if (rain != null)
            rain.Play();
    }

    [ContextMenu("End Storm")]
    public void EndStorm()
    {
        if (phase != StormPhase.Active)
        {
            Debug.LogWarning($"[WeatherController] EndStorm ignored, phase is {phase} (expected Active).", this);
            return;
        }
        if (stormDirector == null || stormExitTimeline == null)
        {
            Debug.LogWarning("[WeatherController] EndStorm ignored, Storm Director or Storm Exit Timeline not assigned.", this);
            return;
        }
        phase = StormPhase.Ended;

        stormDirector.playableAsset = stormExitTimeline;
        stormDirector.time = 0;
        stormDirector.Play();

        // Rain rate ramps down to 0 inside the exit timeline; the system itself is left
        // playing (rate 0 = no new particles) so the Hold extrapolation keeps that state.
    }

    private void SetStormCloudsActive(bool active)
    {
        if (stormClouds == null) return;
        foreach (GameObject go in stormClouds)
            if (go != null)
                go.SetActive(active);
    }
}
