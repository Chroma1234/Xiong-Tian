using UnityEngine;

public class Afterimage : MonoBehaviour
{
    [SerializeField] private float fadeTime = 0.3f;
    [SerializeField] private float startAlpha = 0.3f;

    private SpriteRenderer sr;
    private float timer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        timer = fadeTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Color c = sr.color;
        c.a = (timer / fadeTime) * startAlpha; // fade out
        sr.color = c;
    }

    public void SetSprite(Sprite sprite, Vector3 scale, Color color)
    {
        sr.sprite = sprite;
        transform.localScale = scale;
        sr.color = color;
    }
}
