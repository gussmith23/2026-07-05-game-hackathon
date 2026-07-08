using System.Collections;
using UnityEngine;

public class ArrowTrigger : MonoBehaviour
{
    public GameObject carriedRocketVisual;
    public GameObject placedRocket;
    public Vector3 flyAwayOffset = new Vector3(12f, 12f, 0f);
    public float delayBeforeLaunch = 2f;
    public float flyDuration = 1.5f;

    bool used;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;
        StartCoroutine(PlaceAndLaunch());
    }

    IEnumerator PlaceAndLaunch()
    {
        if (carriedRocketVisual != null) carriedRocketVisual.SetActive(false);
        placedRocket.SetActive(true);

        yield return new WaitForSeconds(delayBeforeLaunch);

        FindFirstObjectByType<WeatherExit>()?.TriggerClear();

        var block = placedRocket.GetComponent<PlaceholderBlock>();
        Vector3 target = placedRocket.transform.position + flyAwayOffset;
        yield return StartCoroutine(block.MoveRoutine(target, flyDuration));
    }
}
