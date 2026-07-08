using UnityEngine;
using UnityEngine.Playables;

// Companion to the submodule's WeatherController: that component owns the storm's
// build-up (EntryDirector, held at peak once played). This owns the separate
// clearing timeline (ExitDirector) the submodule ships but doesn't wire up itself,
// since our story needs an explicit "clear now" moment (the rocket launch) rather
// than a fixed-length storm.
public class WeatherExit : MonoBehaviour
{
    public PlayableDirector exitDirector;

    void Awake()
    {
        if (exitDirector == null) return;
        exitDirector.playOnAwake = false;
        exitDirector.extrapolationMode = DirectorWrapMode.Hold;
        exitDirector.Stop();
    }

    public void TriggerClear()
    {
        if (exitDirector == null) return;
        exitDirector.time = 0;
        exitDirector.Play();
    }
}
