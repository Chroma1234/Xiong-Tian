using UnityEngine;

public class Parallax : MonoBehaviour
{
    #region Parallax 
    public float parallaxEffect;
    private float length;
    private float startPos;
    #endregion

    #region Main Camera Reference
    public Transform cam;
    #endregion

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
    }
}