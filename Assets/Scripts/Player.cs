using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
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
    #endregion

    #region Attack Mechanics
    [Header("Attack Settings")]
    [SerializeField] public float attackBufferTime = 0.15f;
    [HideInInspector] public float attackBufferCounter;
    [SerializeField] private float iFrameDuration = 1f;
    #endregion

    #region Health
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health;

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
    #endregion

    #region Component References
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D playerCollider;
    private BoxCollider2D attackCollider;
    private BoxCollider2D blockCollider;
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

    public PlayerDeadState DeadState { get; set; }
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine);
        FallState = new PlayerFallState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        AttackState = new PlayerAttackState(this, StateMachine);
        BlockState = new PlayerBlockState(this, StateMachine);
        HitState = new PlayerHitState(this, StateMachine);
        DeadState = new PlayerDeadState(this, StateMachine);

        playerLayer = gameObject.layer;
        enemyLayer = LayerMask.NameToLayer("EnemyHitbox");
        groundLayer = LayerMask.GetMask("Ground");

        attackCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        blockCollider = transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        health = maxHealth;
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
        if (StateMachine.CurrentPlayerState != DeadState)
        {
            if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y / 4);
            }

            if (Input.GetMouseButtonDown(0))
            {
                attackBufferCounter = attackBufferTime;
            }

            if (IsGrounded())
            {
                coyoteTimeCounter = coyoteTime;
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
            if (StateMachine.CurrentPlayerState != AttackState && StateMachine.CurrentPlayerState != BlockState && StateMachine.CurrentPlayerState != DashState && StateMachine.CurrentPlayerState != HitState)
            {
                HandleMovement();
                FlipPlayerSprite();
            }

            if (StateMachine.CurrentPlayerState == DashState)
            {
                SpawnAfterimages();
            }

            GetLastFacingDirection();
        }

        HandleAnimators();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhysicsUpdate();
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
        if (StateMachine.CurrentPlayerState != AttackState && StateMachine.CurrentPlayerState != BlockState)
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

    private void AfterAttack()
    {
        StateMachine.ChangeState(IdleState);
    }

    private void Attack()
    {
        StateMachine.ChangeState(AttackState);
    }

    public void TakeHit(int damage, Vector2 hitDirection, float knockbackForce)
    {
        if (!IsAlive || StateMachine.CurrentPlayerState == DeadState)
            return;

        Health -= damage;

        rb.linearVelocity = Vector2.zero;

        Vector2 launchDir = new Vector2(hitDirection.x, 1f).normalized;

        rb.AddForce(launchDir * knockbackForce, ForceMode2D.Impulse);

        // Enter hit state (unless dead)
        if (Health > 0)
        {
            StateMachine.ChangeState(HitState);
        }
    }

    public IEnumerator Invincibility()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        float timer = 0f;

        while (timer < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
            timer += flickerInterval;
        }

        spriteRenderer.enabled = true;
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    private void Die()
    {
        IsAlive = false;

        rb.linearVelocity = Vector2.zero;

        DisablePlayerCollider();
        DisableAttackCollider();
        DisableBlockCollider();

        StateMachine.ChangeState(DeadState);
    }

    public void EnablePlayerCollider()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
    }

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void EnableBlockCollider()
    {
        blockCollider.enabled = true;
    }

    public void DisablePlayerCollider()
    {
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
    }

    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    public void DisableBlockCollider()
    {
        blockCollider.enabled = false;
    }

    public bool IsGrounded()
    {
        //raycast to check if player is in contact with game objects with "Ground" layer
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
}
