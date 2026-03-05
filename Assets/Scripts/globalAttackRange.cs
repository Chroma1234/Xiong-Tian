using System.Collections;
using UnityEngine;

public class globalAttackRange : MonoBehaviour
{

    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        originalColor = this.gameObject.GetComponent<SpriteRenderer>().color;

        //Debug.Log("Range Set");

        //StartCoroutine(Warning());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Warning()
    {
        yield return new WaitForSeconds(1);
    }
}
