using System;
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

    private float gravity = 0f;
    private Vector3 velocityScale;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip impactClip;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();   
        enemyMat = spriteRenderer.material;

        gravity = rb.gravityScale;
        velocityScale = rb.linearVelocity;
    }

    private void OnEnable()
    {
        enemyMat.SetFloat(dissolveAmt, 0);

        rb.gravityScale = gravity;
        rb.linearVelocity = velocityScale;
        rb.simulated = true;

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

            rb.gravityScale = 0f;
            rb.linearVelocity = Vector3.zero;
            rb.simulated = false;

            StartCoroutine(Vanish());
        }
        
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}
