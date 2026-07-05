using UnityEngine;
using UnityEngine.InputSystem;

public class GroundPlayerController : MonoBehaviour
{
    public float speed = 4f;
    public float groundY;

    // Outdoors this is null and movement is horizontal-only, locked to groundY.
    // Indoors (set by House) movement is free within the given rect.
    Rect? indoorBounds;

    // Outdoors/on-roof horizontal-only clamp (e.g. so the player can't walk off the roof edge).
    Vector2? horizontalClamp;

    void Start()
    {
        groundY = transform.position.y;
    }

    public void SetIndoorBounds(Rect? bounds)
    {
        indoorBounds = bounds;
        if (bounds == null) groundY = transform.position.y;
    }

    public void SetHorizontalClamp(Vector2? clamp)
    {
        horizontalClamp = clamp;
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = (kb.dKey.isPressed || kb.rightArrowKey.isPressed ? 1f : 0f)
                - (kb.aKey.isPressed || kb.leftArrowKey.isPressed ? 1f : 0f);

        var pos = transform.position;
        pos.x += h * speed * Time.deltaTime;

        if (indoorBounds.HasValue)
        {
            float v = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
                    - (kb.sKey.isPressed || kb.downArrowKey.isPressed ? 1f : 0f);
            pos.y += v * speed * Time.deltaTime;

            var b = indoorBounds.Value;
            pos.x = Mathf.Clamp(pos.x, b.xMin, b.xMax);
            pos.y = Mathf.Clamp(pos.y, b.yMin, b.yMax);
        }
        else
        {
            pos.y = groundY;
            if (horizontalClamp.HasValue)
                pos.x = Mathf.Clamp(pos.x, horizontalClamp.Value.x, horizontalClamp.Value.y);
        }

        transform.position = pos;
    }
}
