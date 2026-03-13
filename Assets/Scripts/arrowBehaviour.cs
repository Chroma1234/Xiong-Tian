using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class arrowBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            Fade();
            //this.gameObject.SetActive(false);
            Debug.Log("Disabled oneself");
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Fade());
            Debug.Log("Trigger");
        }

        
    }

    private IEnumerator Fade()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 0.75f)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0f, 0, (elapsedTime / 0.75f));

            this.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), lerpedDissolve);

            yield return null;
        }

        this.GetComponent<SpriteRenderer>().material.SetFloat(Shader.PropertyToID("_DissolveAmt"), 0);
    }
}
