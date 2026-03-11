using System.Collections;
using UnityEngine;

public class Room_TrapDoors : MonoBehaviour
{
    CameraController cam;
    [SerializeField] 
    BoxCollider2D room;
    public GameObject blocker;
    public GameObject[] blockers;
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
            StartCoroutine(Vanish());
        }
    }

    private IEnumerator Vanish()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.75f)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / 0.75f));

            foreach(GameObject e in blockers)
            {
                e.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), lerpedDissolve);
            }
            yield return null;
        }

        foreach (GameObject e in blockers)
        {
            e.SetActive(false);
        }
    }
}
