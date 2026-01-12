using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float forwardOffset;
    [SerializeField] private float xFollowSpeed;
    [SerializeField] private float yFollowSpeed;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;

    private float targetX;
    private float targetY;

    private void FixedUpdate()
    {
        targetX = Mathf.Lerp(targetX, (forwardOffset * player.localScale.x), Time.deltaTime * xFollowSpeed);
        targetY = Mathf.Lerp(transform.position.y, player.position.y, Time.deltaTime * yFollowSpeed);
        transform.position = new Vector3(player.position.x + targetX, targetY, transform.position.z);
    }

    public void DoShake()
    {
        StartCoroutine(Shake(shakeDuration, shakeMagnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Use Perlin noise for smoother, less predictable shake
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null; // Wait until the next frame
        }

        transform.localPosition = originalPos; // Return to original position
    }
}
