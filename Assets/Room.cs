using System.Collections;
using UnityEngine;

public class Room : MonoBehaviour
{
    CameraController cam;
    [SerializeField] 
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
        if (enemiesInRoom != null)
        {
            foreach (GameObject e in enemiesInRoom)
            {
                if (e != null && e.activeSelf)
                {
                    return false;
                }
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
        while (elapsedTime < 0.4f)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / 0.4f));

            blocker.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), lerpedDissolve);
            yield return null;
        }
        blocker.SetActive(false);
    }
}
