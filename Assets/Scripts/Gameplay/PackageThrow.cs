using UnityEngine;

// Physics-based toss off the truck bed: random launch angle/power/spin, arcs and
// tumbles under real 3D gravity, lands and settles on the terrain naturally. This is a
// separate 3D Rigidbody/Collider from the existing 2D trigger collider (PackagePickup),
// which keeps working for player pickup regardless of this rigidbody's simulation.
[RequireComponent(typeof(Rigidbody))]
public class PackageThrow : MonoBehaviour
{
    public float minThrowAngle = 25f;
    public float maxThrowAngle = 55f;
    public float minThrowSpeed = 4f;
    public float maxThrowSpeed = 7f;
    public float minSpin = 180f;
    public float maxSpin = 540f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Throw(Vector3 fromPosition)
    {
        transform.position = fromPosition;
        transform.rotation = Quaternion.identity;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float direction = Random.value < 0.5f ? 1f : -1f;
        float angle = Random.Range(minThrowAngle, maxThrowAngle) * Mathf.Deg2Rad;
        float speed = Random.Range(minThrowSpeed, maxThrowSpeed);
        rb.linearVelocity = new Vector3(Mathf.Cos(angle) * direction, Mathf.Sin(angle), 0f) * speed;

        rb.angularVelocity = Random.insideUnitSphere * Random.Range(minSpin, maxSpin) * Mathf.Deg2Rad;
    }
}
