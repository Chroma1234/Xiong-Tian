using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class globalAttackRange : MonoBehaviour
{

    private Color originalColor;
    private SpriteRenderer spriteRenderer;

    public Coroutine warningCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Debug.Log("Range Set");

        //StartCoroutine(Warning());

        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;

        
    }

    // Update is called once per frame
    void Update()
    {
        //StartCoroutine(warningPulse(3));

        if (this.gameObject.activeSelf)
        {
            

            //Fade Coroutine | Target Alpha Value, Time to Target
            //StartCoroutine(FadeTo(1f, 3.0f));
        }

        else if (!this.gameObject.activeSelf)
        {
            spriteRenderer.color = originalColor;

            //Fade Coroutine | Target Alpha Value, Time to Target
            //StopCoroutine(FadeTo(0, 0));
        }

        
    }

    public static void ExecuteAttack()
    {
        //Debug.Log("Beta");
        //warningCoroutine = StartCoroutine(FadeTo(1f, 3.0f));

    }

    

    IEnumerator warningPulse(float duration)
    {
        Color set = Color.red;

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(duration);

            spriteRenderer.color = set;

            yield return new WaitForSeconds(duration);

            spriteRenderer.color = originalColor;
        }
        
        yield return null;
    }
}
