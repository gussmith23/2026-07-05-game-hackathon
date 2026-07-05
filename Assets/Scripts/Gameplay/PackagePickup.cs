using UnityEngine;

public class PackagePickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var carrier = other.GetComponent<PlayerPackageCarrier>();
        if (carrier == null) return;

        carrier.Pickup();
        gameObject.SetActive(false);
    }
}
