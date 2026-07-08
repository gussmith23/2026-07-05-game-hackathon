using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Decorative clouds drift until StartStorm(), then one storm timeline plays the whole sequence
/// on the 8 real clouds: enter -> cover -> light dims -> rain -> hold -> rain stops -> exit -> brighten.
/// All the beat timing lives inside that single timeline.
/// </summary>
[DisallowMultipleComponent]
public class WeatherController : MonoBehaviour
{
    [SerializeField] private AmbientDrift ambient;

    [Tooltip("The 8 storm clouds. Hidden during ambient; revealed when the storm plays.")]
    [SerializeField] private GameObject[] stormClouds;

    [Tooltip("The single storm timeline (enter -> cover -> dim -> rain -> hold -> stop -> exit -> brighten).")]
    [SerializeField] private PlayableDirector stormDirector;

    [Tooltip("Rain particle system. Its emission RATE is animated by the timeline, but the system itself must be told to play — the controller does that when the storm starts.")]
    [SerializeField] private ParticleSystem rain;

    private bool stormStarted;

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
        if (stormStarted || stormDirector == null) return;
        stormStarted = true;

        if (ambient != null) ambient.StopDrift();
        SetStormCloudsActive(true);

        stormDirector.time = 0;
        stormDirector.Play();

        // The system must be playing for the timeline's emission-rate curve to actually emit;
        // it starts at rate 0 and the timeline ramps it up (~t=1.6s) and back to 0.
        if (rain != null)
            rain.Play();
    }

    private void SetStormCloudsActive(bool active)
    {
        if (stormClouds == null) return;
        foreach (GameObject go in stormClouds)
            if (go != null)
                go.SetActive(active);
    }
}
