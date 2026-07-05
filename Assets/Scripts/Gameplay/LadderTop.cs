using UnityEngine;

public class LadderTop : MonoBehaviour
{
    public House house;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        house.ReturnInside(other.gameObject);
    }
}
