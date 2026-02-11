using UnityEngine;

public class Room : MonoBehaviour
{
    CameraController cam;
    BoxCollider2D room;
    public GameObject blocker;

    public GameObject[] enemiesInRoom;

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

    private void Update()
    {
        if (enemiesInRoom.Length > 0)
        {
            if (AllEnemiesInRoomDead())
            {
                DestroyBlocker();
            }
        }
    }

    private bool AllEnemiesInRoomDead()
    {
        foreach(GameObject e in enemiesInRoom)
        {
            if (e.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    public void DestroyBlocker()
    {
        if (blocker != null)
        {
            blocker.SetActive(false);
        }
    }
}
