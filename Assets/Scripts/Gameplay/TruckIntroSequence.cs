using System.Collections;
using UnityEngine;

public class TruckIntroSequence : MonoBehaviour
{
    public GameObject player;
    public DialogBox dialogBox;
    public Vector3 truckStopPosition;
    public Vector3 truckExitPosition;
    public Vector3 playerDropPosition;
    public float driveInDuration = 2f;
    public float driveAwayDuration = 2f;
    [TextArea] public string dialogueText = "Placeholder dialogue text goes here.";

    void Start()
    {
        if (player != null) player.SetActive(false);
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        var truckBlock = GetComponent<PlaceholderBlock>();

        yield return StartCoroutine(truckBlock.MoveRoutine(truckStopPosition, driveInDuration));

        bool waiting = true;
        dialogBox.Show(dialogueText, () => waiting = false);
        yield return new WaitUntil(() => !waiting);

        if (player != null)
        {
            player.transform.position = playerDropPosition;
            player.SetActive(true);
        }

        yield return StartCoroutine(truckBlock.MoveRoutine(truckExitPosition, driveAwayDuration));
    }
}
