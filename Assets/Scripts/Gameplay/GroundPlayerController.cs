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

    Animator animator;
    Transform modelTransform;

    void Start()
    {
        groundY = transform.position.y;
        animator = GetComponentInChildren<Animator>();
        if (animator != null) modelTransform = animator.transform;
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

        float v = 0f;
        var pos = transform.position;
        pos.x += h * speed * Time.deltaTime;

        if (indoorBounds.HasValue)
        {
            v = (kb.wKey.isPressed || kb.upArrowKey.isPressed ? 1f : 0f)
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

        if (animator != null)
            animator.SetBool("IsMoving", Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f);

        // The rig's default forward (from the glTF/Blender export) faces +Z, straight at
        // the camera, instead of sideways along the walk axis -- so face +X/-X based on
        // the last horizontal direction moved, leaving orientation unchanged while idle.
        if (modelTransform != null && Mathf.Abs(h) > 0.01f)
            modelTransform.localRotation = Quaternion.Euler(0f, h > 0f ? 90f : -90f, 0f);
    }
}
