using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    private Camera cam;

    #region Player Reference
    [Header("Player Reference")]
    [SerializeField] private Transform player;
    private Player playerScript;
    #endregion

    #region Camera Follow Settings
    [Header("Camera Follow Settings")]
    [SerializeField] private float forwardOffset;
    [SerializeField] private float heightOffset;
    [SerializeField] private float xFollowSpeed;
    [SerializeField] private float yFollowSpeed;

    private float targetX;
    private float targetY;
    #endregion

    #region Zoom Settings
    [Header("Zoom Settings")]
    [SerializeField] private float zoomDuration = 0.3f;
    [SerializeField] private float zoomAmount = 2f;
    #endregion

    #region Shake Settings
    private Vector3 shakeOffset;
    #endregion

    #region Room Settings
    [Header("Room Settings")]
    [SerializeField] private float roomTransitionDuration = 0.1f;
    private BoxCollider2D bounds;
    private bool isTransitioning = false;
    #endregion

    #region Post Processing
    [Header("Post Processing")]
    [SerializeField] private Volume volume;
    private Vignette vignette;
    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    #endregion

    #region Misc
    private float originalSize;
    private float targetSize;

    private float originalDistortion;
    private float originalVignette;
    private float originalAberration;

    public bool inCutscene = false;
    #endregion

    private void Awake()
    {
        cam = Camera.main;
        playerScript = player.GetComponent<Player>();

        originalSize = cam.fieldOfView;
        targetSize = originalSize / zoomAmount;
    }

    private void Start()
    {
        if (volume.profile.TryGet<LensDistortion>(out lensDistortion))
        {
            lensDistortion.active = true;
            lensDistortion.intensity.overrideState = true;
        }

        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.active = true;
            vignette.intensity.overrideState = true;
        }

        if (volume.profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.active = true;
            chromaticAberration.intensity.overrideState = true;
        }

        originalDistortion = lensDistortion.intensity.value;
        originalVignette = vignette.intensity.value;
        originalAberration = chromaticAberration.intensity.value;
    }

    private void Update()
    {
        if (isTransitioning || inCutscene)
            return;

        targetX = Mathf.Lerp(targetX, (forwardOffset * player.localScale.x), Time.deltaTime * xFollowSpeed);
        targetY = Mathf.Lerp(transform.position.y, player.position.y + heightOffset, Time.deltaTime * yFollowSpeed);

        float distance = Mathf.Abs(transform.position.z - player.position.z);
        float camHalfHeight = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
        float camHalfWidth = camHalfHeight * cam.aspect;

        float minX = bounds.bounds.min.x + camHalfWidth;
        float maxX = bounds.bounds.max.x - camHalfWidth;
        float minY = bounds.bounds.min.y + camHalfHeight;
        float maxY = bounds.bounds.max.y - camHalfHeight;

        float clampedX = Mathf.Clamp(player.position.x + targetX, minX, maxX);
        float clampedY = Mathf.Clamp(targetY, minY, maxY);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z) + shakeOffset;
    }

    public void DoShake(float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    public void ZoomIn()
    {
        StopCoroutine("ZoomCoroutine");
        StartCoroutine(ZoomCoroutine());
    }

    private IEnumerator ZoomCoroutine()
    {
        float elapsed = 0f;
        float elapsedPP = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            elapsedPP += Time.unscaledDeltaTime;
            cam.fieldOfView = Mathf.Lerp(originalSize, targetSize, elapsed / zoomDuration);

            float fraction = Mathf.Clamp01(elapsedPP / zoomDuration * 2f);
            lensDistortion.intensity.value = Mathf.SmoothStep(originalDistortion, -0.3f, fraction);
            vignette.intensity.value = Mathf.SmoothStep(originalVignette, 0.5f, fraction);
            chromaticAberration.intensity.value = Mathf.SmoothStep(originalAberration, 0.3f, fraction);

            yield return null;
        }

        cam.fieldOfView = targetSize;

        elapsed = 0f;
        elapsedPP = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(targetSize, originalSize, elapsed / zoomDuration);

            lensDistortion.intensity.value = Mathf.SmoothStep(-0.3f, originalDistortion, elapsed / zoomDuration);
            vignette.intensity.value = Mathf.SmoothStep(0.5f, originalVignette, elapsed / zoomDuration);
            chromaticAberration.intensity.value = Mathf.SmoothStep(0.3f, originalAberration, elapsed / zoomDuration);

            yield return null;
        }

        cam.fieldOfView = originalSize;
    }

    public void MoveToRoom(BoxCollider2D room)
    {
        StopCoroutine("RoomTransition");
        StartCoroutine(RoomTransition(room));
    }

    private IEnumerator RoomTransition(BoxCollider2D newRoom)
    {
        isTransitioning = true;
        playerScript.isTeleporting = true;
        playerScript.rb.linearVelocity = Vector3.zero;
        BoxCollider2D oldRoom = bounds;
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        float distance = Mathf.Abs(transform.position.z - player.position.z);
        float camHalfHeight = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
        float camHalfWidth = camHalfHeight * cam.aspect;

        float minX = newRoom.bounds.min.x + camHalfWidth;
        float maxX = newRoom.bounds.max.x - camHalfWidth;
        float minY = newRoom.bounds.min.y + camHalfHeight;
        float maxY = newRoom.bounds.max.y - camHalfHeight;

        float endX = Mathf.Clamp(player.position.x + (forwardOffset * player.localScale.x), minX, maxX);
        float endY = Mathf.Clamp(player.position.y + heightOffset, minY, maxY);
        Vector3 endPos = new Vector3(endX, endY, transform.position.z);

        while (elapsed < roomTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / roomTransitionDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        targetX = forwardOffset * player.localScale.x;
        targetY = endY;

        bounds = newRoom;
        transform.position = endPos;
        isTransitioning = false;
        playerScript.isTeleporting = false;
    }
}
