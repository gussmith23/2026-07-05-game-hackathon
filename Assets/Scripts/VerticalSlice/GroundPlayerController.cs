using UnityEngine;
using UnityEngine.InputSystem;

public class GroundPlayerController : MonoBehaviour
{
    public float speed = 4f;
    public float groundY;

    void Start()
    {
        groundY = transform.position.y;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);

        var pos = transform.position;
        pos.x += h * speed * Time.deltaTime;
        pos.y = groundY;
        transform.position = pos;
    }
}
