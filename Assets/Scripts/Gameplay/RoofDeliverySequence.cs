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
    public float packageDropDuration = 1.2f;

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

        var truckBlock = truck.GetComponent<PlaceholderBlock>();
        Vector3 truckRestPos = truck.transform.position;
        Vector3 truckStopPos = new Vector3(packageRestPosition.x, truckRestPos.y, truckRestPos.z);

        yield return StartCoroutine(truckBlock.MoveRoutine(truckStopPos, truckTravelDuration));

        var packageBlock = package.GetComponent<PlaceholderBlock>();
        yield return StartCoroutine(packageBlock.MoveRoutine(packageRestPosition, packageDropDuration));

        yield return StartCoroutine(truckBlock.MoveRoutine(truckRestPos, truckTravelDuration));
    }
}
