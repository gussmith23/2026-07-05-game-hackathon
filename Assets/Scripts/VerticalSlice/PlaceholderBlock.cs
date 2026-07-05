using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PlaceholderBlock : MonoBehaviour
{
    public Color color = Color.white;
    public Vector2 size = new Vector2(1f, 1f);

    private static Sprite _squareSprite;

    void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sprite = GetSquareSprite();
        sr.color = color;
        transform.localScale = new Vector3(size.x, size.y, 1f);
    }

    static Sprite GetSquareSprite()
    {
        if (_squareSprite == null)
        {
            var tex = new Texture2D(4, 4);
            var pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            _squareSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        }
        return _squareSprite;
    }

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
