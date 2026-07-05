using UnityEngine;
using System.Collections;

// Pure runtime behavior only. Visual appearance (sprite, color, size, position)
// is static data on the SpriteRenderer/Transform, set once in the scene/build script
// and visible in Edit mode without this script needing to run.
public class PlaceholderBlock : MonoBehaviour
{
    public IEnumerator MoveRoutine(Vector3 target, float duration)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float f = Mathf.Clamp01(t / duration);
            f = f * f * (3f - 2f * f); // smoothstep ease
            transform.position = Vector3.Lerp(start, target, f);
            yield return null;
        }
        transform.position = target;
    }
}
