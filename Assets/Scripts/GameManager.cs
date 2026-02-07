using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region FPS
    [Header("FPS Control")]
    public int fps;
    #endregion

    #region Hitstop
    [Header("Hitstop Settings")]
    public static GameManager instance;
    private float defaultTimeScale = 1f;
    private float hitStopTimeScale = 0f;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = fps;
    }

    public void DoHitStop(float duration)
    {
        StartCoroutine(HitStop(duration));
    }

    public void DoSlowDown(float factor, float duration)
    {
        StartCoroutine(SlowMotion(factor, duration));
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = hitStopTimeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = defaultTimeScale;
    }

    IEnumerator SlowMotion(float factor, float duration)
    {
        Time.timeScale = factor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

}
