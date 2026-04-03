using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Events
    public static event Action<Enemy> OnEnemyHit;
    public static event Action<Enemy> OnEnemyKilled;
    #endregion

    #region Reset Settings
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private GameManager manager;
    #endregion

    #region Component References
    public GameObject player;
    [SerializeField] private LayerMask playerLayer;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    private Material enemyMat;
    public Animator animator;
    BoxCollider2D boxCollider;
    #endregion

    #region Health Settings
    [Header("Health Settings")]
    [SerializeField] private int maxHealth;
    [HideInInspector] private int health;
    #endregion

    #region Damaged Settings
    [Header("Damaged Settings")]
    [SerializeField] public float knockbackForce = 8f;
    [SerializeField] public float stunDuration = 2f;

    [SerializeField] private float hitFlashDuration;
    #endregion

    #region Chase Settings
    [Header("Chase Settings")]
    public Vector2 leftLimit;
    public Vector2 rightLimit;
    public bool canMove = true;

    [HideInInspector] public bool inAttackRange = false;
    Vector2 lastPosition;
    public Transform groundCheck;
    #endregion

    #region VFX / Particles
    [Header("VFX / Particles")]
    [SerializeField] private GameObject stunnedStarsPrefab;
    [SerializeField] private GameObject sparksPrefab;
    private GameObject stars;
    public ParticleSystem eyeFlash;

    [SerializeField] private float dissolveTime = 0.5f;
    private int dissolveAmt = Shader.PropertyToID("_DissolveAmt");
    #endregion

    #region Audio
    [Header("Audio")]
    [SerializeField] private AudioClip hit;
    [SerializeField] private AudioClip shieldHit;
    private AudioSource audioSource;
    #endregion

    #region Shield Dissolve Settings
    [Header("Shield Dissolve Settings")]
    private int shieldDissolveAmt = Shader.PropertyToID("_DissolveAmt");
    [SerializeField] public GameObject shieldObject;
    private SpriteRenderer shieldRenderer;
    #endregion

    public bool shieldType = false;

    #region States
    public PawnStateMachine StateMachine { get; set; }
    public PawnIdleState IdleState { get; set; }
    public PawnChaseState ChaseState { get; set; }
    public PawnAttackState AttackState { get; set; }
    public PawnStunnedState StunnedState { get; set; }
    public PawnDeadState DeadState { get; set; }
    #endregion

    public Vector2 FacingDirection
    {
        get => transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);

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
        }

        Vector2 launchDir = new Vector2(-hitDirection.x, 0f).normalized;
        StartCoroutine(Knockback(launchDir));

        OnEnemyHit?.Invoke(this);

        if (Health > 0 && damage != 0)
        {
            StartCoroutine(FlashSprite());
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMat = spriteRenderer.material;

        StateMachine = new PawnStateMachine();
        IdleState = new PawnIdleState(this, StateMachine);
        ChaseState = new PawnChaseState(this, StateMachine);
        AttackState = new PawnAttackState(this, StateMachine);
        StunnedState = new PawnStunnedState(this, StateMachine, stunDuration); 
        DeadState = new PawnDeadState(this, StateMachine);

        //Getting the Positions of the pathfinding Limits
        leftLimit = transform.GetChild(2).gameObject.transform.position;
        rightLimit = transform.GetChild(3).gameObject.transform.position;

        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        manager = FindFirstObjectByType<GameManager>();
        manager.RegisterEnemy(this);

        if (shieldObject != null)
        {
            shieldRenderer = shieldObject.GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);

        health = maxHealth;

        triggerShield();
    }

    private void Update()
    {
        StateMachine.CurrentPawnState.FrameUpdate();

        Vector2 direction = transform.localScale.x == -1 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, playerLayer);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && StateMachine.CurrentPawnState == ChaseState && StateMachine.CurrentPawnState != DeadState)
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
        StateMachine.CurrentPawnState.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
      
        if(obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentPawnState != ChaseState && StateMachine.CurrentPawnState != DeadState && StateMachine.CurrentPawnState != AttackState && StateMachine.CurrentPawnState != StunnedState)
        {
            player = obj;
            StateMachine.ChangeState(ChaseState);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;

        if (obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentPawnState != ChaseState && StateMachine.CurrentPawnState != DeadState && StateMachine.CurrentPawnState != AttackState && StateMachine.CurrentPawnState != StunnedState)
        {
            player = obj;
            StateMachine.ChangeState(ChaseState);
        }
    }

    public void OnAttackFinished()
    {
        StateMachine.CurrentPawnState?.OnAttackFinished();
    }

    private IEnumerator FlashSprite()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = Color.white;
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
        if (shieldRenderer != null)
        {
            StartCoroutine(ShieldVanish(0f, 1.1f));
        }

        if (transform.localScale.x == 1)
        {
            stars = Instantiate(stunnedStarsPrefab, transform.position + new Vector3(0.2f, 0.4f, 0), Quaternion.Euler(90, 0, 0), transform);
        }
        else
        {
            stars = Instantiate(stunnedStarsPrefab, transform.position + new Vector3(-0.2f, 0.4f, 0), Quaternion.Euler(90, 0, 0), transform);
        }
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

    public void HandleAnimators()
    {
        Vector2 currentPosition = transform.position;
        bool isMoving = Vector2.Distance(currentPosition, lastPosition) > 0.001f;
        animator.SetBool("isWalking", isMoving);
        lastPosition = currentPosition;
    }

    public void Die()
    {
        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            if(col != boxCollider)
            col.enabled = false;
        }

        if(stars != null)
        {
            Destroy(stars);
        }

        IsAlive = false;
        OnEnemyKilled?.Invoke(this);
        animator.SetTrigger("death");
        StateMachine.ChangeState(DeadState);

        StartCoroutine(Vanish());
    }

    public void ResetEnemy()
    {
        StopAllCoroutines();
        enemyMat.SetFloat(dissolveAmt, 0f);
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        
        if (shieldRenderer != null)
        {
            shieldRenderer.material.SetFloat(shieldDissolveAmt, 0f);
        }

        Collider2D[] cols = GetComponentsInChildren<Collider2D>();
        foreach (var col in cols)
        {
            if (col != boxCollider && col.name != "NormalAtkHitbox" && col.name != "ParryAtkHitbox")
            {
                col.enabled = true;
            }
        }
        Health = maxHealth;
        IsAlive = true;
        StateMachine.ChangeState(IdleState);
        animator.Rebind();
        animator.Update(0f);
        gameObject.SetActive(true);
    }

    private IEnumerator Vanish()
    {
        yield return new WaitForSeconds(0.5f);
        float elapsedTime = 0f;
        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0, 1.1f, (elapsedTime / dissolveTime));

            enemyMat.SetFloat(dissolveAmt, lerpedDissolve);

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void triggerShield()
    {
        if (shieldType)
        {
            StartCoroutine(ShieldVanish(1.1f, 0f));
        }
    }

    public IEnumerator ShieldVanish(float from, float to)
    {
        if (StateMachine.CurrentPawnState != null)
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
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}

