using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class arrowBehaviour : MonoBehaviour
{
    private float dissolveTime = 0.75f;
    private int dissolveAmt = Shader.PropertyToID("_DissolveAmt");

    private SpriteRenderer spriteRenderer;
    private Material enemyMat;
    private Rigidbody2D rb;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip impactClip;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();   
        enemyMat = spriteRenderer.material;
    }

    private IEnumerator Vanish()
    {  
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / dissolveTime));

            enemyMat.SetFloat(dissolveAmt, lerpedDissolve);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
        {
            PlaySound(impactClip);
            //this.gameObject.SetActive(false); 
            //this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector3.zero;
            StartCoroutine(Vanish());
            //Debug.Log("Disabled oneself");
        }
        
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}
