using UnityEngine;

public class Workbench : MonoBehaviour
{
    public PuzzleUI puzzleUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var carrier = other.GetComponent<PlayerPackageCarrier>();
        if (carrier == null || !carrier.HasPackage) return;

        carrier.Consume();
        puzzleUI.Open();
    }
}
