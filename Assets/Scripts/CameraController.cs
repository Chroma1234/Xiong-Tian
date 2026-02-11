using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;

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

    private void Awake()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        targetX = Mathf.Lerp(targetX, (forwardOffset * player.localScale.x), Time.deltaTime * xFollowSpeed);
        targetY = Mathf.Lerp(transform.position.y, player.position.y + heightOffset, Time.deltaTime * yFollowSpeed);

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

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
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void ZoomIn()
    {
        StopCoroutine("ZoomCoroutine");
        StartCoroutine(ZoomCoroutine());
    }

    private IEnumerator ZoomCoroutine()
    {
        float originalSize = cam.fieldOfView;
        float targetSize = originalSize / zoomAmount;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(originalSize, targetSize, elapsed / zoomDuration);
            yield return null;
        }

        cam.fieldOfView = targetSize;

        elapsed = 0f;
        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(targetSize, originalSize, elapsed / zoomDuration);
            yield return null;
        }

        cam.fieldOfView = originalSize;
    }

    public void MoveToRoom(BoxCollider2D room)
    {
        bounds = room;
    }
}
