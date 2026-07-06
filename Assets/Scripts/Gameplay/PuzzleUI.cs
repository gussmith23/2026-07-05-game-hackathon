using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    public GameObject panelRoot;
    public PuzzlePiece[] pieces;
    public GameObject carriedRocketVisual;
    public GameObject arrow;

    int placedCount;

    void Awake()
    {
        foreach (var p in pieces) p.OnPlaced += HandlePiecePlaced;
        panelRoot.SetActive(false);
    }

    public void Open()
    {
        placedCount = 0;
        // Activate first: pieces are nested under panelRoot, which starts inactive, so their
        // Awake() (and the RectTransform it caches) hasn't run until the panel is shown.
        panelRoot.SetActive(true);
        foreach (var p in pieces) p.ResetPiece();
    }

    void HandlePiecePlaced()
    {
        placedCount++;
        if (placedCount >= pieces.Length)
        {
            if (carriedRocketVisual != null) carriedRocketVisual.SetActive(true);
            if (arrow != null) arrow.SetActive(true);
            Invoke(nameof(Close), 0.6f);
        }
    }

    void Close()
    {
        panelRoot.SetActive(false);
    }
}
