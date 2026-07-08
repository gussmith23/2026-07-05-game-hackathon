using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Dev trigger: an on-screen IMGUI button (no Canvas/EventSystem needed) plus the Space key,
/// both calling WeatherController.StartStorm(). Replace with your own game logic / UI Button
/// (wire its OnClick to StartStorm) when ready.
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
            Fire();
#else
        if (Input.GetKeyDown(KeyCode.Space))
            Fire();
#endif
    }

    private void OnGUI()
    {
        GUI.skin.button.fontSize = 20;
        if (GUI.Button(new Rect(20, 20, 220, 60), "Start Storm  (Space)"))
            Fire();
    }

    private void Fire()
    {
        if (weather != null)
            weather.StartStorm();
        else
            Debug.LogWarning("[StormTrigger] No WeatherController assigned.", this);
    }
}
