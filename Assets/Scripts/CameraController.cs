using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float forwardOffset;
    [SerializeField] private float heightOffset;
    [SerializeField] private float xFollowSpeed;
    [SerializeField] private float yFollowSpeed;

    [SerializeField] private float zoomDuration = 0.3f;
    [SerializeField] private float zoomAmount = 2f;

    private float targetX;
    private float targetY;

    [SerializeField] private BoxCollider2D bounds;
    [SerializeField] private float transitionSpeed = 3f;

    private void FixedUpdate()
    {
        targetX = Mathf.Lerp(targetX, (forwardOffset * player.localScale.x), Time.deltaTime * xFollowSpeed);
        targetY = Mathf.Lerp(transform.position.y, player.position.y + heightOffset, Time.deltaTime * yFollowSpeed);

        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;

        float minX = bounds.bounds.min.x + camHalfWidth;
        float maxX = bounds.bounds.max.x - camHalfWidth;
        float minY = bounds.bounds.min.y + camHalfHeight;
        float maxY = bounds.bounds.max.y - camHalfHeight;

        float clampedX = Mathf.Clamp(player.position.x + targetX, minX, maxX);
        float clampedY = Mathf.Clamp(targetY, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    public void DoShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
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

    public void ZoomIn()
    {
        StopCoroutine("ZoomCoroutine");
        StartCoroutine(ZoomCoroutine());
    }

    private IEnumerator ZoomCoroutine()
    {
        float originalSize = Camera.main.orthographicSize;
        float targetSize = originalSize / zoomAmount;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Lerp(originalSize, targetSize, elapsed / zoomDuration);
            yield return null;
        }

        Camera.main.orthographicSize = targetSize;

        // Hold zoom briefly
        //yield return new WaitForSeconds(0.2f);

        elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Lerp(targetSize, originalSize, elapsed / zoomDuration);
            yield return null;
        }

        Camera.main.orthographicSize = originalSize;
    }

    public void MoveToRoom(BoxCollider2D room)
    {
        bounds = room;
    }
}
