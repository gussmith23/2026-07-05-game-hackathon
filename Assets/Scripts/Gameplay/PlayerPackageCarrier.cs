using UnityEngine;

public class PlayerPackageCarrier : MonoBehaviour
{
    public GameObject carriedVisual;

    public bool HasPackage { get; private set; }

    public void Pickup()
    {
        HasPackage = true;
        if (carriedVisual != null) carriedVisual.SetActive(true);
    }

    public void Consume()
    {
        HasPackage = false;
        if (carriedVisual != null) carriedVisual.SetActive(false);
    }
}
