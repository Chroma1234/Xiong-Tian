using UnityEngine;

public class SoulOrb : MonoBehaviour
{
    [Header("Targets")]
    public Transform target;

    [Header("Timing")]
    public float delayBeforeHoming = 0.15f;
    public float spiralDuration = 0.4f;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float spiralRadius = 0.5f;
    public float spiralSpeed = 10f;
    public float homingStrength = 8f;

    private float timer;
    private float angle;
    private Vector2 baseDirection;

    private Vector2 origin;

    private void Start()
    {
        origin = transform.position;
        angle = Random.Range(0f, 360f);

        if (target != null)
        {
            baseDirection = (target.position - transform.position).normalized;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < delayBeforeHoming)
            return;

        if (timer < delayBeforeHoming + spiralDuration)
        {
            SpiralMove();
        }
        else
        {
            HomeIn();
        }
    }

    private void SpiralMove()
    {
        angle += spiralSpeed * Time.deltaTime;

        Vector2 spiralOffset = new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ) * spiralRadius;

        origin += baseDirection * moveSpeed * Time.deltaTime;

        transform.position = origin + spiralOffset;
    }

    private void HomeIn()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        baseDirection = Vector2.Lerp(baseDirection, dir, homingStrength * Time.deltaTime);
        transform.position += (Vector3)(baseDirection * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // TODO: grant soul / resource
        Destroy(gameObject);
    }
}
