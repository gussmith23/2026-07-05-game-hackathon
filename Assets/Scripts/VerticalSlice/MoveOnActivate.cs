using UnityEngine;

public class MoveOnActivate : MonoBehaviour
{
    public PlaceholderBlock block;
    public Vector3 targetPosition;
    public float duration = 1f;

    public void Play()
    {
        if (block == null) block = GetComponent<PlaceholderBlock>();
        StartCoroutine(block.MoveRoutine(targetPosition, duration));
    }
}
