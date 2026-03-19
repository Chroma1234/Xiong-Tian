using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private float startPos;

    public Transform cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        if (GetComponent<SpriteRenderer>() != null)
        {
            length = GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }

    void LateUpdate()
    {
        float dist = cam.position.x * parallaxEffect;
        float temp = cam.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        //if (temp > startPos + length)
        //{
        //    startPos += length * 2;
        //}
        //else if (temp < startPos - length)
        //{
        //    startPos -= length * 2;
        //}
    }
}