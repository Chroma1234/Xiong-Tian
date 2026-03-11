using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Trap_Detection : MonoBehaviour
{

    [SerializeField] 
    public BoxCollider2D trap_detection;
    public GameObject trapdoor;

    private void Awake()
    {
        trap_detection = GetComponent<BoxCollider2D>();
        trapdoor.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            trap_detection.enabled = false;
            StartCoroutine(Appear());           
        }
    }

    
    private IEnumerator Appear()
    {
        trapdoor.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < 0.75f)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.1f, 0, (elapsedTime / 0.75f));

            trapdoor.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), lerpedDissolve);

            yield return null;
        }
        trapdoor.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), 0);
    }
}
