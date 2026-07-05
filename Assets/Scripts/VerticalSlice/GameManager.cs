using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Beat[] beatOrder = new Beat[]
    {
        Beat.WakeUp,
        Beat.PackageArrives,
        Beat.AssembleRocket,
        Beat.FireRocket,
        Beat.Weather
    };

    private int currentIndex = 0;

    public bool IsComplete => currentIndex >= beatOrder.Length;

    // Once complete, reports the final beat rather than throwing; no interaction point matches it anymore.
    public Beat CurrentBeat => beatOrder[Mathf.Min(currentIndex, beatOrder.Length - 1)];

    void Awake()
    {
        Instance = this;
    }

    // Amy's weather controller subscribes here to react when a beat completes.
    public event System.Action<Beat> BeatCompleted;

    public bool AdvanceBeat(Beat beat)
    {
        if (currentIndex >= beatOrder.Length) return false;
        if (beat != CurrentBeat) return false;

        BeatCompleted?.Invoke(beat);
        currentIndex++;

        // Weather has no interaction point of its own; it auto-completes right after FireRocket.
        if (currentIndex < beatOrder.Length && beatOrder[currentIndex] == Beat.Weather)
        {
            BeatCompleted?.Invoke(Beat.Weather);
            currentIndex++;
        }

        return true;
    }
}
