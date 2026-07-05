using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform target;
    public float snapDistance = 40f;

    public event System.Action OnPlaced;

    RectTransform rect;
    Canvas canvas;
    Vector2 startAnchoredPos;
    bool placed;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startAnchoredPos = rect.anchoredPosition;
    }

    public void ResetPiece()
    {
        placed = false;
        rect.anchoredPosition = startAnchoredPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placed) return;
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placed) return;
        if (Vector2.Distance(rect.anchoredPosition, target.anchoredPosition) <= snapDistance)
        {
            rect.anchoredPosition = target.anchoredPosition;
            placed = true;
            OnPlaced?.Invoke();
        }
    }
}
