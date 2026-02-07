using UnityEngine;

public class Room : MonoBehaviour
{
    CameraController cam;
    BoxCollider2D room;
    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraController>();
        room = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cam.MoveToRoom(room);
        }
    }
}
