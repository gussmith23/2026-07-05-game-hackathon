using System.Collections;
using UnityEngine;

public class ArrowTrigger : MonoBehaviour
{
    public GameObject carriedRocketVisual;
    public GameObject placedRocket;
    public float delayBeforeLaunch = 2f;

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

        FindFirstObjectByType<WeatherController>()?.EndStorm();
        placedRocket.GetComponent<RocketFlight>().Launch();
    }
}
