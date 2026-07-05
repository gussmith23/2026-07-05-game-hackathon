using UnityEngine;

enum HouseState { Outside, Inside, OnRoof }

[RequireComponent(typeof(BoxCollider2D))]
public class House : MonoBehaviour
{
    public GameObject interiorVisual;
    public Rect interiorBounds;
    public Vector3 roofPosition;
    public Vector2 roofHorizontalClamp;
    public Vector3 returnInsidePosition;

    // How far right the player can walk while inside, past the interior wall and out through
    // the house's own trigger boundary, so OnTriggerExit2D fires naturally on the same
    // collider used to enter — no separate exit trigger or hand-tuned clearance needed.
    public float exitReachX;
    public float outsideGroundY = -4.4f;

    HouseState state = HouseState.Outside;

    public event System.Action<GameObject> PlayerReachedRoof;

    Rect MovementBounds => new Rect(interiorBounds.xMin, interiorBounds.yMin,
        exitReachX - interiorBounds.xMin, interiorBounds.height);

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only a genuine walk-up from outside should trigger entry — touching the same
        // trigger volume while already inside or standing on the roof is a no-op.
        if (state != HouseState.Outside || !other.CompareTag("Player")) return;
        EnterHouse(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // The player walking out through the right wall leaves this same trigger volume —
        // that's the natural signal to go back outside.
        if (state != HouseState.Inside || !other.CompareTag("Player")) return;
        ExitOutside(other.gameObject);
    }

    void EnterHouse(GameObject player)
    {
        state = HouseState.Inside;
        GetComponent<MeshRenderer>().enabled = false;
        interiorVisual.SetActive(true);

        // Appear just inside whichever wall the player approached from, at the same height,
        // rather than warping to a fixed spot.
        Vector3 approachPos = player.transform.position;
        bool fromLeft = approachPos.x < transform.position.x;
        float enterX = fromLeft ? interiorBounds.xMin : interiorBounds.xMax;
        float enterY = Mathf.Clamp(approachPos.y, interiorBounds.yMin, interiorBounds.yMax);
        player.transform.position = new Vector3(enterX, enterY, 0f);

        var controller = player.GetComponent<GroundPlayerController>();
        controller?.SetHorizontalClamp(null);
        controller?.SetIndoorBounds(MovementBounds);
    }

    public void ExitToRoof(GameObject player)
    {
        if (state != HouseState.Inside) return;

        state = HouseState.OnRoof;
        GetComponent<MeshRenderer>().enabled = true;
        interiorVisual.SetActive(false);

        player.transform.position = roofPosition;

        var controller = player.GetComponent<GroundPlayerController>();
        controller?.SetIndoorBounds(null);
        controller?.SetHorizontalClamp(roofHorizontalClamp);

        PlayerReachedRoof?.Invoke(player);
    }

    public void ReturnInside(GameObject player)
    {
        if (state != HouseState.OnRoof) return;

        state = HouseState.Inside;
        GetComponent<MeshRenderer>().enabled = false;
        interiorVisual.SetActive(true);

        player.transform.position = returnInsidePosition;

        var controller = player.GetComponent<GroundPlayerController>();
        controller?.SetHorizontalClamp(null);
        controller?.SetIndoorBounds(MovementBounds);
    }

    void ExitOutside(GameObject player)
    {
        state = HouseState.Outside;
        GetComponent<MeshRenderer>().enabled = true;
        interiorVisual.SetActive(false);

        // Keep whatever X they naturally walked out to; only snap Y back down to normal
        // ground level (indoor Y can differ, since the room floor isn't the same height).
        var pos = player.transform.position;
        player.transform.position = new Vector3(pos.x, outsideGroundY, 0f);

        var controller = player.GetComponent<GroundPlayerController>();
        controller?.SetIndoorBounds(null);
    }
}
