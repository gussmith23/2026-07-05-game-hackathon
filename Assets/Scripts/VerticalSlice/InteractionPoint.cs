using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractionPoint : MonoBehaviour
{
    public Beat beat;
    public MoveOnActivate[] actionsToPlay;
    public GameObject promptRoot;

    private bool playerInRange;

    void Reset()
    {
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        bool isActiveBeat = GameManager.Instance != null && GameManager.Instance.CurrentBeat == beat;
        bool showPrompt = playerInRange && isActiveBeat;

        if (promptRoot != null && promptRoot.activeSelf != showPrompt)
            promptRoot.SetActive(showPrompt);

        if (showPrompt && Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            Activate();
        }
    }

    void Activate()
    {
        if (actionsToPlay != null)
        {
            foreach (var a in actionsToPlay)
            {
                if (a != null) a.Play();
            }
        }
        GameManager.Instance.AdvanceBeat(beat);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
