using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Dev trigger: on-screen IMGUI buttons (no Canvas/EventSystem needed) plus Space/Backspace keys,
/// calling WeatherController.StartStorm()/EndStorm(). Replace with your own game logic / UI Buttons
/// (wire OnClick to StartStorm/EndStorm) when ready.
/// </summary>
public class StormTrigger : MonoBehaviour
{
    [SerializeField] private WeatherController weather;

    private void Reset()
    {
        weather = FindFirstObjectByType<WeatherController>();
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            FireStart();
        if (Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame)
            FireEnd();
#else
        if (Input.GetKeyDown(KeyCode.Space))
            FireStart();
        if (Input.GetKeyDown(KeyCode.Backspace))
            FireEnd();
#endif
    }

    private void OnGUI()
    {
        GUI.skin.button.fontSize = 20;
        if (GUI.Button(new Rect(20, 20, 220, 60), "Start Storm  (Space)"))
            FireStart();
        if (GUI.Button(new Rect(20, 90, 220, 60), "End Storm  (Backspace)"))
            FireEnd();
    }

    private void FireStart()
    {
        if (weather != null)
            weather.StartStorm();
        else
            Debug.LogWarning("[StormTrigger] No WeatherController assigned.", this);
    }

    private void FireEnd()
    {
        if (weather != null)
            weather.EndStorm();
        else
            Debug.LogWarning("[StormTrigger] No WeatherController assigned.", this);
    }
}
