using UnityEngine;

public class PackagePickup : MonoBehaviour
{
    // The object to hide on pickup. Left unset, defaults to this GameObject -- but the
    // trigger collider may live on a child (2D colliders can't share a GameObject with a
    // 3D Rigidbody, needed on the package root for its physics throw), in which case this
    // should point at the actual visible package.
    public GameObject packageRoot;

    // Keep the trigger upright regardless of the package's own tumble: a BoxCollider2D's
    // effective footprint is derived from its full 3D world transform, so if the parent
    // (rotated freely by PackageThrow's physics) tips onto its side, the collider's world
    // shape can shrink to near nothing -- decoupling rotation here keeps pickup reliable
    // no matter how the package lands.
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var carrier = other.GetComponent<PlayerPackageCarrier>();
        if (carrier == null) return;

        carrier.Pickup();
        (packageRoot != null ? packageRoot : gameObject).SetActive(false);
        FindFirstObjectByType<WeatherController>()?.StartStorm();
    }
}
