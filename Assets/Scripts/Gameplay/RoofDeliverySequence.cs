using System.Collections;
using UnityEngine;

public class RoofDeliverySequence : MonoBehaviour
{
    public House house;
    public GameObject truck;
    public GameObject package;
    public Vector3 packageRestPosition;
    public float delayBeforeDelivery = 2f;
    public float truckTravelDuration = 2f;
    public float throwHeightOffset = 1.5f;
    public float throwSettleDuration = 1.5f;

    bool hasTriggered;

    void Awake()
    {
        house.PlayerReachedRoof += OnPlayerReachedRoof;
    }

    void OnPlayerReachedRoof(GameObject player)
    {
        if (hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(Deliver());
    }

    IEnumerator Deliver()
    {
        yield return new WaitForSeconds(delayBeforeDelivery);

        // Facing whichever way it exited after the intro, so it drives in nose-first
        // for this second appearance instead of arriving in reverse.
        truck.transform.Rotate(0f, 180f, 0f);

        var truckBlock = truck.GetComponent<PlaceholderBlock>();
        Vector3 truckRestPos = truck.transform.position;
        Vector3 truckStopPos = new Vector3(packageRestPosition.x, truckRestPos.y, truckRestPos.z);

        yield return StartCoroutine(truckBlock.MoveRoutine(truckStopPos, truckTravelDuration));

        var packageThrow = package.GetComponent<PackageThrow>();
        Vector3 throwOrigin = truckStopPos + new Vector3(0f, throwHeightOffset, 0f);
        package.SetActive(true);
        packageThrow.Throw(throwOrigin);
        yield return new WaitForSeconds(throwSettleDuration);

        yield return StartCoroutine(truckBlock.MoveRoutine(truckRestPos, truckTravelDuration));
    }
}
