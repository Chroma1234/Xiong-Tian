using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour, IDamageable
{
    public event System.Action<int> OnHealthChanged;
    public event System.Action<int> OnManaChanged;

    #region Component References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D playerCollider;
    private PolygonCollider2D attackCollider;
    private BoxCollider2D blockCollider;
    private GameManager gameManager;
    private CameraController cam;
    #endregion

    #region Movement Mechanics
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 10f;
    [SerializeField] public float jumpForce = 17f;
    [SerializeField] public float dodgeSpeed = 15f;
    [SerializeField] public float dodgeDuration = 0.2f;

    [HideInInspector] public Vector2 lastFacingDirection = Vector2.right;
    [HideInInspector] public Vector2 moveDirection;

    [SerializeField] public float coyoteTime = 0.1f;
    [HideInInspector] public float coyoteTimeCounter;

    [SerializeField] public bool doubleJump;
    [HideInInspector] public bool hasDoubleJumped;
    #endregion

    #region Attack Mechanics
    [Header("Attack Settings")]
    public float attackMomentum;
    [SerializeField] public float attackBufferTime = 0.15f;
    [HideInInspector] public float attackBufferCounter;
    [SerializeField] private float iFrameDuration = 1f;
    [SerializeField] private float attackCameraShakeDuration;
    [SerializeField] private float attackCameraShakeMagnitude;
    [SerializeField] private float attackHitstopDuration;
    #endregion

    #region Health Mechanics
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int health;

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
    #endregion

    #region Mana Mechanics
    [SerializeField] private int maxMana;
    [SerializeField] private int mana;
    [SerializeField] public int healingManaCost;
    [SerializeField] public int healingAmt;

    public int Mana
    {
        get => mana;
        set
        {
            mana = Mathf.Clamp(value, 0, maxMana);
            OnManaChanged?.Invoke(mana);
        }
    }

    [SerializeField] private int manaOnHit;
    [SerializeField] private int manaOnKill;
    #endregion

    #region Stamina Settings
    [Header("Stamina Settings")]
    public int dashCount;
    [SerializeField] public int maxDashCount;
    [SerializeField] public float dashRecoveryTime;
    [SerializeField] public float dashRecoveryDelay;
    private float dashRecoveryTimer;
    [HideInInspector] public bool canRecover = true;
    #endregion

    #region Parry Settings
    [Header("Parry Settings")]
    [HideInInspector] public bool parry = false;
    private Coroutine parryCoroutine;
    [SerializeField] private float parryWindow;
    public bool hasParryCharge;
    [SerializeField] private float parryChargeDuration;
    private float parryChargeEndTime;
    [SerializeField] private float parryCameraShakeDuration;
    [SerializeField] private float parryCameraShakeMagnitude;
    [SerializeField] private float parrySlowdownFactor;
    [SerializeField] private float parrySlowdownDuration;
    [SerializeField] private GameObject parrySparksPrefab;
    [SerializeField] private GameObject parryChargeFX;
    #endregion

    #region Layer Masks
    private LayerMask groundLayer;
    private int playerLayer;
    private int enemyLayer;
    #endregion

    #region Effects
    [Header("Effects")]
    [SerializeField] public GameObject afterimagePrefab;
    [SerializeField] public float afterimageSpawnRate = 0.05f;
    [HideInInspector] public float afterimageTimer = 0f;
    [SerializeField] private float flickerInterval = 0.1f;
    [SerializeField] private GameObject healingFxPrefab;
    #endregion

    #region States
    public PlayerStateMachine StateMachine { get; set; }
    public PlayerIdleState IdleState { get; set; }
    public PlayerFallState FallState { get; set; }
    public PlayerDashState DashState { get; set; }
    public PlayerJumpState JumpState { get; set; }
    public PlayerAttackState AttackState { get; set; }
    public PlayerBlockState BlockState { get; set; }
    public PlayerHitState HitState { get; set; }
    public PlayerHealState HealState { get; set; }
    public PlayerDeadState DeadState { get; set; }
    #endregion

    #region SFX
    [Header("SFX")]
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip attackClip;
    public AudioClip parryClip;
    public AudioClip impactClip;
    public AudioClip hitClip;
    public AudioClip healClip;
    public AudioClip deadClip;
    public AudioClip saveClip;
    #endregion

    private int collisionDisableCount = 0;
    public bool isTeleporting = false;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cam = GameObject.Find("Main Camera").GetComponent<CameraController>();

        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine);
        FallState = new PlayerFallState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);
        BlockState = new PlayerBlockState(this, StateMachine);
        HitState = new PlayerHitState(this, StateMachine);
        HealState = new PlayerHealState(this, StateMachine);
        DeadState = new PlayerDeadState(this, StateMachine);

        playerLayer = gameObject.layer;
        enemyLayer = LayerMask.NameToLayer("EnemyHitbox");
        groundLayer = LayerMask.GetMask("Ground");

        attackCollider = transform.GetChild(0).GetComponent<PolygonCollider2D>();
        blockCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        health = maxHealth;
        mana = maxMana;
        StateMachine.Initialize(IdleState);

        Enemy.OnEnemyHit += RecoverManaOnHit;
        Enemy.OnEnemyHit += EnemyHitEffects;
        Enemy.OnEnemyKilled += RecoverManaOnKill;
        Enemy.OnEnemyKilled += EnemyHitEffects;
    }

    private void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
        if (StateMachine.CurrentPlayerState != DeadState)
        {
            if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.25f);
            }

            if (IsGrounded())
            {
                coyoteTimeCounter = coyoteTime;
                hasDoubleJumped = false;    

                if (Input.GetKeyDown(KeyCode.E) && StateMachine.CurrentPlayerState != HealState && StateMachine.CurrentPlayerState != DashState)
                {
                    if (mana >= healingManaCost)
                    {
                        if (Health < maxHealth)
                        {
                            StateMachine.ChangeState(HealState);
                        }
                    }
                    else
                    {
                        //insufficient mana / full health!
                    }
                }
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            attackBufferCounter -= Time.deltaTime;

            if (attackBufferCounter > 0f && StateMachine.CurrentPlayerState != AttackState)
            {
                Attack();
                attackBufferCounter = 0f;
            }

            if (StateMachine.CurrentPlayerState == DashState)
            {
                SpawnAfterimages();
            }

            if (!canRecover)
            {
                dashRecoveryTimer -= Time.deltaTime;

                if (dashRecoveryTimer <= 0f)
                {
                    canRecover = true;
                }
            }

            if (dashCount < maxDashCount && canRecover)
            {
                dashRecoveryTimer += Time.deltaTime;

                if (dashRecoveryTimer >= dashRecoveryTime)
                {
                    dashCount++;
                    dashRecoveryTimer = 0f;
                }
            }

            GetLastFacingDirection();
        }

        if((hasParryCharge && Time.time >= parryChargeEndTime) || !hasParryCharge)
        {
            hasParryCharge = false;
            parryChargeFX.SetActive(false);
            parryChargeFX.GetComponent<ParticleSystem>().Stop();
        }

        HandleAnimators();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhysicsUpdate();

        if (StateMachine.CurrentPlayerState != AttackState && StateMachine.CurrentPlayerState != BlockState && StateMachine.CurrentPlayerState != DashState && StateMachine.CurrentPlayerState != HitState && StateMachine.CurrentPlayerState != HealState && StateMachine.CurrentPlayerState != DeadState && !isTeleporting)
        {
            HandleMovement();
            FlipPlayerSprite();
        }
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyHit -= RecoverManaOnHit;
        Enemy.OnEnemyHit -= EnemyHitEffects;
        Enemy.OnEnemyKilled -= RecoverManaOnKill;
        Enemy.OnEnemyKilled -= EnemyHitEffects;
    }

    private void RecoverManaOnHit(Enemy enemy)
    {
        if (Mana <= maxMana - manaOnHit)
        {
            Mana += manaOnHit;
        }
    }

    private void RecoverManaOnKill(Enemy enemy)
    {
        if (Mana <= maxMana - manaOnKill)
        {
            Mana += manaOnKill;
        }
    }

    public void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    public void HandleAnimators()
    {
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        if (StateMachine.CurrentPlayerState != AttackState && StateMachine.CurrentPlayerState != BlockState && !isTeleporting)
        {
            animator.SetBool("isMoving", Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
            animator.SetBool("isRolling", StateMachine.CurrentPlayerState == DashState);
        animator.SetBool("isBlocking", StateMachine.CurrentPlayerState == BlockState);
        animator.SetBool("isAlive", StateMachine.CurrentPlayerState != DeadState);
    }

    private void GetLastFacingDirection()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection != Vector2.zero)
        {
            lastFacingDirection = moveDirection;
        }
    }

    private void FlipPlayerSprite()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Attack()
    {
        StateMachine.ChangeState(AttackState);
    }

    public void TakeHit(int damage, Vector2 hitDirection, float knockbackForce)
    {
        if (!IsAlive || StateMachine.CurrentPlayerState == DeadState || StateMachine.CurrentPlayerState == HitState)
            return;

        Health -= damage;

        rb.linearVelocity = Vector2.zero;

        Vector2 launchDir = new Vector2(hitDirection.x, 1f).normalized;

        rb.AddForce(launchDir * knockbackForce, ForceMode2D.Impulse);

        cam.DoShake(attackCameraShakeMagnitude, attackCameraShakeDuration);

        // Enter hit state (unless dead)
        if (Health > 0)
        {
            audioSource.PlayOneShot(hitClip);
            StateMachine.ChangeState(HitState);
        }
    }

    public IEnumerator Invincibility()
    {
        DisablePlayerCollider();
        float timer = 0f;

        while (timer < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }

        spriteRenderer.enabled = true;
        EnablePlayerCollider();
    }

    public IEnumerator ParryWindow()
    {
        parry = true;
        yield return new WaitForSeconds(parryWindow);
        parry = false;
    }

    public void Parry()
    {
        cam.DoShake(parryCameraShakeMagnitude, parryCameraShakeDuration);
        cam.ZoomIn();
        gameManager.DoSlowDown(parrySlowdownFactor, parrySlowdownDuration);

        GameObject sparks = Instantiate(parrySparksPrefab, blockCollider.transform.position, Quaternion.identity);
        ParticleSystem ps = sparks.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(sparks, ps.main.duration + ps.main.startLifetime.constantMax);

        hasParryCharge = true;
        parryChargeFX.SetActive(true);
        parryChargeFX.GetComponent<ParticleSystem>().Play();
        parryChargeEndTime = Time.time + parryChargeDuration;
    }

    public void EnemyHitEffects(Enemy enemy)
    {
        cam.DoShake(attackCameraShakeMagnitude, attackCameraShakeDuration);
        gameManager.DoHitStop(attackHitstopDuration);
    }

    public void SaveEffects()
    {
        GameObject heal = Instantiate(healingFxPrefab, transform.position, Quaternion.Euler(-90, 0, 0), transform);
        ParticleSystem ps = heal.GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(heal, ps.main.duration + ps.main.startLifetime.constantMax);
    }

    private void Die()
    {
        IsAlive = false;
        StopAllCoroutines();

        rb.linearVelocity = Vector2.zero;

        DisablePlayerCollider();
        DisableAttackCollider();
        DisableBlockCollider();

        audioSource.PlayOneShot(deadClip);

        StateMachine.ChangeState(DeadState);
        gameManager.RespawnPlayer();
    }

    public void Respawn(Vector3 spawnPoint)
    {
        StopAllCoroutines();

        IsAlive = true;
        Health = maxHealth;
        Mana = maxMana;

        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint;

        EnablePlayerCollider();
        EnableBlockCollider();
        DisableAttackCollider();

        StateMachine.ChangeState(IdleState);
    }

    public void EnablePlayerCollider()
    {
        collisionDisableCount--;
        if (collisionDisableCount <= 0)
        {
            collisionDisableCount = 0;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }
    }

    public void EnableBlockCollider()
    {
        blockCollider.enabled = true;
        parryCoroutine = StartCoroutine(ParryWindow());
    }

    public void DisablePlayerCollider()
    {
        collisionDisableCount++;
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    public void DisableBlockCollider()
    {
        blockCollider.enabled = false;
        if(parryCoroutine != null)
        {
            StopCoroutine(parryCoroutine);
        }
        parry = false;
    }

    public void StartDashRecovery()
    {
        dashRecoveryTimer = dashRecoveryTime;
        canRecover = false;
    }
    public bool IsGrounded()
    {
        //raycast to check if player is on the ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private void SpawnAfterimages()
    {
        afterimageTimer -= Time.deltaTime;
        if (afterimageTimer <= 0f)
        {
            GameObject ghost = Instantiate(afterimagePrefab, transform.position, Quaternion.identity);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            Afterimage afterimage = ghost.GetComponent<Afterimage>();
            afterimage.SetSprite(sr.sprite, transform.localScale, Color.white);
            afterimageTimer = afterimageSpawnRate;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
