using UnityEngine;

// Real physics launch: kinematic while placed, switches to dynamic on Launch() with an
// initial kick, then applies continuous thrust each FixedUpdate while the engine burns.
// Thrust magnitude and heading both wander via Perlin noise -- a "puttering" imprecise
// engine rather than a single impulse coasting under gravity (which reads as a plain
// parabola). Rotation is frozen from physics and instead driven manually to face the
// current velocity direction, so it noses into its own wobbly path.
[RequireComponent(typeof(Rigidbody2D))]
public class RocketFlight : MonoBehaviour
{
    public float minLaunchAngle = 55f;
    public float maxLaunchAngle = 80f;
    public float minLaunchSpeed = 3f;
    public float maxLaunchSpeed = 5f;
    public float gravityScale = 0.6f;

    public float minThrust = 8f;
    public float maxThrust = 14f;
    public float thrustNoiseFrequency = 1.5f;
    public float headingWanderDegrees = 12f;
    public float headingWanderFrequency = 0.6f;
    public float burnDuration = 4f;

    Rigidbody2D rb;
    bool flying;
    float launchTime;
    float headingAngle;
    float noiseSeedA;
    float noiseSeedB;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Launch()
    {
        float angle = Random.Range(minLaunchAngle, maxLaunchAngle);
        float speed = Random.Range(minLaunchSpeed, maxLaunchSpeed);
        headingAngle = angle;
        noiseSeedA = Random.Range(0f, 100f);
        noiseSeedB = Random.Range(0f, 100f);

        float rad = angle * Mathf.Deg2Rad;
        Vector2 velocity = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * speed;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = gravityScale;
        rb.linearVelocity = velocity;
        flying = true;
        launchTime = Time.time;

        var trail = GetComponentInChildren<ParticleSystem>(true);
        if (trail != null) trail.Play();
    }

    void FixedUpdate()
    {
        if (!flying) return;

        if (Time.time - launchTime < burnDuration)
        {
            float wander = (Mathf.PerlinNoise(noiseSeedA, Time.time * headingWanderFrequency) - 0.5f) * 2f * headingWanderDegrees;
            float rad = (headingAngle + wander) * Mathf.Deg2Rad;
            Vector2 thrustDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            float noise = Mathf.PerlinNoise(noiseSeedB, Time.time * thrustNoiseFrequency);
            float thrust = Mathf.Lerp(minThrust, maxThrust, noise);
            rb.AddForce(thrustDir * thrust);
        }

        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude < 0.0001f) return;
        float visualAngle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, visualAngle);
    }
}
