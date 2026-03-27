using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    public static event Action<Boss> OnBossHit;
    public static event Action<Boss> OnBossKilled;

    public event System.Action<int> OnHealthChanged;

    [SerializeField] CameraController cam;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private GameManager manager;

    [HideInInspector] public SpriteRenderer spriteRenderer;
    public Animator animator;
    BoxCollider2D boxCollider;

    [SerializeField] private int maxHealth;
    [HideInInspector] private int health;

    [SerializeField] public float knockbackForce = 8f;

    [SerializeField] public float stunDuration = 2f;
    [SerializeField] private GameObject stunnedStarsPrefab;

    [SerializeField] private float hitFlashDuration;
    [SerializeField] private GameObject sparksPrefab;

    [HideInInspector] public bool inAttackRange = false;
    [SerializeField] private bool moveTest;
    Vector2 lastPosition;

    [SerializeField] private LayerMask playerLayer;
    private GameObject stars;
    public ParticleSystem eyeFlash;

    private AudioSource audioSource;
    [SerializeField] AudioSource bgm;
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip shieldHit;
    [SerializeField] private AudioClip normalBGM;

    //raptor-x-z
    public GameObject player;
    public Vector2 leftLimit;
    public Vector2 rightLimit;
    [HideInInspector] public bool canMove = true;

    [HideInInspector] public bool triggerGlobal = false;
    [HideInInspector] public bool exitGlobal = false;
    [HideInInspector] public bool globalEven = true;

    [HideInInspector] public Vector2 originalPos;

    [HideInInspector] public List<GameObject> warningList = new List<GameObject>();
    [HideInInspector] public List<GameObject> arrowList = new List<GameObject>();

    public GameObject warningObject;
    public GameObject arrowObject;

    public bool triggerLeft = false;

    public float warningArrowOffset = 40f;
    public float globalAttackCooldown = 10f;
    public float globalAttackTimer = 5f;
    public float warningFadeIn = 3f;
    public float warningFadeOut = 2f;

    public float arrowGravity = 40;

    [SerializeField] private float vanishDuration = 0.5f;
    [SerializeField] private float reappearDelay = 0.2f;
    [SerializeField] private BoxCollider2D teleportArea;

    [HideInInspector] public bool globalCooldown = false;
    [HideInInspector] public int globalCounter = 0;

    [SerializeField] private AudioClip arrowFallingClip;
    public Vector2 FacingDirection
    {
        get => transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    public Transform groundCheck;
    public bool justStunned = false;

    [HideInInspector] public bool justTeleported = false;
    [SerializeField] private float teleportLockTime = 0.3f; // time boss cannot attack after teleport

    [SerializeField] private GameObject caveBG;
    [SerializeField] private SpriteRenderer[] bossBGSprites;

    [SerializeField] private float dissolveTime = 0.75f;
    private int shieldDissolveAmt = Shader.PropertyToID("_DissolveAmt");
    [SerializeField] public ParticleSystemRenderer shieldRenderer;

    #region States
    public BossStateMachine StateMachine { get; set; }
    public BossEnterState EnterState { get; set; }
    public BossIdleState IdleState { get; set; }
    public BossChaseState ChaseState { get; set; }
    public BossAttackState AttackState { get; set; }
    public BossStunnedState StunnedState { get; set; }
    public BossDeadState DeadState { get; set; }

    public BossGlobalAttackState GlobalAttackState { get; set; }
    #endregion

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(health);

            if (health <= 0 && IsAlive)
            {
                Die();
            }
        }
    }

    public bool IsAlive { get; set; } = true;

    public void TakeHit(int damage, Vector2 hitDirection, float knockbackForce, bool blocked)
    {
        if (!IsAlive)
            return;

        Health -= damage;

        GameObject sparks = Instantiate(sparksPrefab, transform.position, Quaternion.identity);
        ParticleSystem ps = sparks.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(sparks, ps.main.duration + ps.main.startLifetime.constantMax);

        if (blocked)
        {
            audioSource.PlayOneShot(shieldHit);
        }
        else
        {
            audioSource.PlayOneShot(hit);

            if (Health > 0)
            {
                StartCoroutine(FlashSprite());
            }
        }

        //Vector2 launchDir = new Vector2(-hitDirection.x, 0f).normalized;
        //StartCoroutine(Knockback(launchDir));

        OnBossHit?.Invoke(this);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StateMachine = new BossStateMachine();
        EnterState = new BossEnterState(this, StateMachine);
        IdleState = new BossIdleState(this, StateMachine);
        ChaseState = new BossChaseState(this, StateMachine);
        AttackState = new BossAttackState(this, StateMachine);
        StunnedState = new BossStunnedState(this, StateMachine, stunDuration);
        DeadState = new BossDeadState(this, StateMachine);
        GlobalAttackState = new BossGlobalAttackState(this, StateMachine);

        //raptor-x-z
        //Getting the Positions of the pathfinding Limits
        leftLimit = transform.GetChild(2).gameObject.transform.position;
        rightLimit = transform.GetChild(3).gameObject.transform.position;

        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        manager = FindFirstObjectByType<GameManager>();

        originalPos = transform.position;

        triggerGlobal = false;
        exitGlobal = false;
        //Debug.Log(warningArray.Length);

        float warningPosX = -20f;
        float arrowPosX = -20f;

        //Instantiate warning blocking
        for (int i = 0; i < 40; i++)
        {
            GameObject newPrefab = Instantiate(warningObject, new Vector2(this.transform.position.x + warningPosX, 43.5f + warningArrowOffset), Quaternion.identity);

            newPrefab.gameObject.SetActive(false);

            warningList.Add(newPrefab);
            warningPosX += 1.5f;
        }

        //Instantiate arrows positions
        for (int i = 0; i < 40; i++)
        {
            GameObject newPrefab = Instantiate(arrowObject, new Vector2(this.transform.position.x + arrowPosX, this.transform.position.y + 4f), Quaternion.identity);

            newPrefab.gameObject.SetActive(false);

            newPrefab.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

            newPrefab.gameObject.GetComponent<Rigidbody2D>().gravityScale = arrowGravity;

            arrowList.Add(newPrefab);
            arrowPosX += 1.5f;
        }

    }

    private void Start()
    {
        StateMachine.Initialize(EnterState);

        health = maxHealth;
    }

    private void Update()
    {
        StateMachine.CurrentBossState.FrameUpdate();

        Vector2 direction = transform.localScale.x == -1 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.2f), direction, 1.5f, playerLayer);
        
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.2f), direction * 1.5f, Color.green);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && StateMachine.CurrentBossState == ChaseState && StateMachine.CurrentBossState != DeadState && StateMachine.CurrentBossState != GlobalAttackState)
            {
                inAttackRange = true;
            }
        } 
        else
        {
            inAttackRange = false;
        }

        HandleAnimators();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentBossState.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentBossState != ChaseState && StateMachine.CurrentBossState != DeadState && StateMachine.CurrentBossState != AttackState && StateMachine.CurrentBossState != StunnedState && StateMachine.CurrentBossState != GlobalAttackState && !triggerGlobal && StateMachine.CurrentBossState != EnterState)
        {
            player = obj;
            StateMachine.ChangeState(ChaseState);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentBossState != ChaseState && StateMachine.CurrentBossState != DeadState && StateMachine.CurrentBossState != AttackState && StateMachine.CurrentBossState != StunnedState && StateMachine.CurrentBossState != GlobalAttackState && !triggerGlobal && StateMachine.CurrentBossState != EnterState)
        {
            player = obj;
            StateMachine.ChangeState(ChaseState);
        }

        if (StateMachine.CurrentBossState != DeadState && StateMachine.CurrentBossState != AttackState && StateMachine.CurrentBossState != StunnedState && StateMachine.CurrentBossState != GlobalAttackState && triggerGlobal && StateMachine.CurrentBossState != EnterState)
        {
            //Debug.Log("Trigger Global: " + boss.triggerGlobal);         

            StateMachine.ChangeState(GlobalAttackState);
            animator.SetTrigger("dipped");

        }
    }

    //Temp to exit GLobal State
    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentBossState == GlobalAttackState && StateMachine.CurrentBossState != ChaseState && StateMachine.CurrentBossState != DeadState && StateMachine.CurrentBossState != AttackState && StateMachine.CurrentBossState != StunnedState && !triggerGlobal && StateMachine.CurrentBossState != EnterState)
        {
            player = obj;
            triggerGlobal = false;
            exitGlobal = false;
            //StateMachine.ChangeState(IdleState);
        }

        if (obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentBossState == ChaseState && StateMachine.CurrentBossState != GlobalAttackState && StateMachine.CurrentBossState != DeadState && StateMachine.CurrentBossState != AttackState && StateMachine.CurrentBossState != StunnedState && !triggerGlobal && StateMachine.CurrentBossState != EnterState)
        {
            player = obj;
            //StateMachine.ChangeState(IdleState);
        }
    }

    public void OnAttackFinished()
    {
        StateMachine.CurrentBossState?.OnAttackFinished();
    }

    public void StartAttack()
    {
        GlobalAttackState.OnVanishComplete();
    }

    private IEnumerator FlashSprite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    public void EyeFlash()
    {
        if (eyeFlash != null)
        {
            eyeFlash.Play();
        }
    }

    public void Stunned()
    {
        StartCoroutine(ShieldVanish(0f, 1.1f));
        if (transform.localScale.x == 1)
            stars = Instantiate(stunnedStarsPrefab, transform.position + new Vector3(-0.15f, 1.8f, 0), Quaternion.Euler(90, 0, 0), transform);
        else
            stars = Instantiate(stunnedStarsPrefab, transform.position + new Vector3(0.15f, 1.8f, 0), Quaternion.Euler(90, 0, 0), transform);
        ParticleSystem ps = stars.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(stars, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    public void ParryKnockback(Vector2 hitDirection)
    {
        Vector2 launchDir = new Vector2(-hitDirection.x, 0f).normalized;
        StartCoroutine(Knockback(launchDir));
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        float timer = 0f;

        while (timer < 0.1f)
        {
            timer += Time.deltaTime;
            if (direction != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, knockbackForce * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void Teleport()
    {
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        justTeleported = true;
        canMove = false;
        animator.SetTrigger("vanish");
        yield return new WaitForSeconds(vanishDuration);
        transform.position = GetRandomPointInCollider(teleportArea);
        yield return new WaitForSeconds(reappearDelay);
        animator.SetTrigger("reappear");
        yield return new WaitForSeconds(teleportLockTime);
        StateMachine.ChangeState(GlobalAttackState);
        justTeleported = false;
        canMove = true;
        animator.ResetTrigger("vanish");
        animator.ResetTrigger("reappear");
    }

    private Vector2 GetRandomPointInCollider(BoxCollider2D box)
    {
        Vector2 size = box.size;
        Vector2 center = box.offset + (Vector2)box.transform.position;

        float minDistance = 5f; // minimum distance from player
        Vector2 playerPos = player.transform.position;

        Vector2 randomPos;
        int attempts = 0;

        do
        {
            float randomX = UnityEngine.Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
            randomPos = new Vector2(randomX, transform.position.y);
            attempts++;
        }
        while (Vector2.Distance(randomPos, playerPos) < minDistance && attempts < 20);

        return randomPos;
    }

    public void HandleAnimators()
    {
        Vector2 currentPosition = transform.position;
        bool isMoving = Vector2.Distance(currentPosition, lastPosition) > 0.001f;
        animator.SetBool("isWalking", isMoving);
        lastPosition = currentPosition;
    }

    public void Die()
    {
        StopAllCoroutines();
        manager.DoSlowDown(0.025f, 0.5f);
        cam.ZoomIn();
        StartCoroutine(FadeAudio(0f, 1f));

        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            if (col != boxCollider)
                col.enabled = false;
        }

        if (stars != null)
        {
            Destroy(stars);
        }
        IsAlive = false;
        OnBossKilled?.Invoke(this);
        animator.SetTrigger("death");
        StateMachine.ChangeState(DeadState);

        StartCoroutine(DisableAfterTime(1f));
    }

    public void ResetEnemy()
    {
        StopAllCoroutines();
        StateMachine.ChangeState(EnterState);
        shieldRenderer.material.SetFloat(shieldDissolveAmt, 1f);
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            if (col != boxCollider && col.name != "Hitbox" && !triggerGlobal)
                col.enabled = true;
        }
        Health = maxHealth;
        IsAlive = true;
        animator.Rebind();
        animator.Update(0f);
        gameObject.SetActive(true);

        StartCoroutine(FadeAudio(0f, 1f));
        bgm.Stop();
        bgm.clip = normalBGM;
        caveBG.SetActive(true);

        foreach (SpriteRenderer renderer in bossBGSprites)
        {
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

        bgm.Play();
        StartCoroutine(FadeAudio(0.1f, 1f));
    }


    public IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    //Condition to trigger global attack
    public IEnumerator GlobalTimer()
    {
        Debug.Log("Started Timer");

        //Time to trigger Global Attack State without any interactions
        yield return new WaitForSeconds(globalAttackTimer);

        Debug.Log("Timer Ended");

        if (!globalCooldown)
        {
            triggerGlobal = true;
        }

    }

    //Fade Coroutine
    public IEnumerator FadeTo(float targetAlpha, float duration, SpriteRenderer render)
    {
        float startAlpha = render.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float blend = Mathf.Clamp01(time / duration);

            Color newColor = new Color(render.color.r, render.color.g, render.color.b, Mathf.Lerp(startAlpha, targetAlpha, blend));

            render.color = newColor;

            yield return null;
        }
    }

    public IEnumerator GlobalStateCooldown()
    {
        Debug.Log("Cooldown Triggered");
        globalCooldown = true;

        yield return new WaitForSeconds(globalAttackCooldown);
        Debug.Log("Cooldown ended");

        globalCooldown = false;
    
    }

    //Trigger Arrows Coroutine
    public IEnumerator triggerArrows()
    {
        Debug.Log("Triggering arrowAttacks function");

        //Delay
        yield return new WaitForSeconds(warningFadeIn + 0.2f);

        //Starting Arrows Attack
        //Fade out warning pillars
        for (int i = 0; i < warningList.Count; i++)
        {
            if (warningList[i].gameObject.activeSelf)
            {
                GameObject stanceObject = warningList[i];

                SpriteRenderer stanceObjRender = stanceObject.GetComponent<SpriteRenderer>();

                Color newColor = Color.red;
                stanceObjRender.color = newColor;

                //Coroutine to fade out object
                StartCoroutine(FadeTo(0f, warningFadeOut, stanceObjRender));
                
            }
            
        }

        yield return new WaitForSeconds(warningFadeOut + 0.2f);

        //Triggers Arrow Rain
        Debug.Log("Raining Arrows");
        PlaySound(arrowFallingClip);
        for (int i = 0; i < arrowList.Count; i++)
        {
            if (warningList[i].gameObject.activeSelf)
            {
                arrowList[i].transform.position = new Vector2(
            arrowList[i].transform.position.x,   // keep X the same
            transform.position.y + 4f           // reset Y to boss spawn height
            );
                Rigidbody2D rb = arrowList[i].GetComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Dynamic;
                arrowList[i].SetActive(true);
                arrowList[i].gameObject.SetActive(true);
                warningList[i].gameObject.SetActive(false);
            }

            arrowList[i].gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }

        //globalCounter++;

        //yield return new WaitForSeconds(1f);

        for (int wave = 0; wave < 5; wave++)
        {
            // spawn arrows
            yield return new WaitForSeconds(0.2f);
        }
        StateMachine.ChangeState(ChaseState);
        StartCoroutine(ShieldVanish(1.1f, 0f));
        triggerGlobal = false;
        globalCounter = 0;

        //if (globalCounter <= 2)
        //{
        //    globalCounter = 0;

        //    triggerGlobal = false;
        //    exitGlobal = false;

        //    StateMachine.ChangeState(IdleState);
        //}
    }

    private IEnumerator FadeAudio(float targetVolume, float duration)
    {
        float startVolume = bgm.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgm.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        bgm.volume = targetVolume;
    }

    public void ResetWarnings()
    {
        for (int i = 0; i < warningList.Count; i++)
        {
            warningList[i].SetActive(true);

            SpriteRenderer sr = warningList[i].GetComponent<SpriteRenderer>();
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }

    public IEnumerator ShieldVanish(float from, float to)
    {
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(from, to, (elapsedTime / dissolveTime));

            shieldRenderer.material.SetFloat(shieldDissolveAmt, lerpedDissolve);
            yield return null;
        }
    }

    public IEnumerator Entrance()
    {
        animator.SetTrigger("enter");
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsTag("Entrance")
        );

        yield return new WaitUntil(() =>
            !animator.GetCurrentAnimatorStateInfo(0).IsTag("Entrance")
        );

        StateMachine.ChangeState(ChaseState);
        StartCoroutine(ShieldVanish(1.1f, 0f));
    }

}
