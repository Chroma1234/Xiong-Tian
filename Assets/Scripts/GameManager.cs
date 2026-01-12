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
    private float hitStopDuration = 0.05f;
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

    public void DoHitStop()
    {
        StartCoroutine(HitStopCoroutine());
    }

    IEnumerator HitStopCoroutine()
    {
        Time.timeScale = hitStopTimeScale;
        // Wait for the duration using real-time seconds, unaffected by timeScale
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = defaultTimeScale;
    }

}
