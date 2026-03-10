using UnityEngine;

public class LockDoor : MonoBehaviour
{
    BoxCollider2D room;
    public GameObject ForestLockDoor;

    private void Awake()
    {
        room = GetComponent<BoxCollider2D>();
        ForestLockDoor = GetComponent<GameObject>();
        //ForestLocked.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //cam.MoveToRoom(room);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
