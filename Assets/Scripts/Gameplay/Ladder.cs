using UnityEngine;

public class Ladder : MonoBehaviour
{
    public House house;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        house.ExitToRoof(other.gameObject);
    }
}
