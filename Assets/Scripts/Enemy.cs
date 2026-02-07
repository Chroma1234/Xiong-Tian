using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public static event Action<Enemy> OnEnemyHit;
    public static event Action<Enemy> OnEnemyKilled;

    [HideInInspector] public SpriteRenderer spriteRenderer;
    public Animator animator;
    BoxCollider2D boxCollider;

    [SerializeField] private int maxHealth = 100;
    [HideInInspector] private int health;

    [SerializeField] public float knockbackForce = 8f;

    [SerializeField] public float stunDuration = 2f;
    [SerializeField] private GameObject stunnedStarsPrefab;

    [SerializeField] private float hitFlashDuration;

    [HideInInspector] public bool inAttackRange = false;
    [SerializeField] private bool moveTest;
    Vector2 lastPosition;

    [SerializeField] private LayerMask playerLayer;
    private int enemyLayer;

    //raptor-x-z
    public GameObject player;
    public Vector3 leftLimit;
    public Vector3 rightLimit;
    public bool canMove = true;

    #region States
    public PawnStateMachine StateMachine { get; set; }
    public PawnIdleState IdleState { get; set; }
    public PawnChaseState ChaseState { get; set; }
    public PawnAttackState AttackState { get; set; }
    public PawnStunnedState StunnedState { get; set; }
    public PawnDeadState DeadState { get; set; }
    #endregion

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

    public void TakeHit(int damage, Vector2 hitDirection, float knockbackForce)
    {
        if (!IsAlive)
            return;

        Health -= damage;

        Vector2 launchDir = new Vector2(-hitDirection.x, 0f).normalized;
        StartCoroutine(Knockback(launchDir));

        OnEnemyHit?.Invoke(this);

        if (Health > 0)
        {
            StartCoroutine(FlashSprite());
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StateMachine = new PawnStateMachine();
        IdleState = new PawnIdleState(this, StateMachine);
        ChaseState = new PawnChaseState(this, StateMachine);
        AttackState = new PawnAttackState(this, StateMachine);
        StunnedState = new PawnStunnedState(this, StateMachine, stunDuration); 
        DeadState = new PawnDeadState(this, StateMachine);

        //raptor-x-z
        //Getting the Positions of the pathfinding Limits
        leftLimit = transform.GetChild(2).gameObject.transform.position;
        rightLimit = transform.GetChild(3).gameObject.transform.position;

        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        enemyLayer = LayerMask.NameToLayer("EnemyHitbox");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);

        health = maxHealth;
    }

    private void Update()
    {
        if (moveTest)
        {
            transform.Translate(Vector3.left * 5f * Time.deltaTime);
        }

        StateMachine.CurrentPawnState.FrameUpdate();

        Vector2 direction = transform.localScale.x == -1 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, playerLayer);
        Debug.DrawRay(transform.position, direction * 1f, Color.green);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && StateMachine.CurrentPawnState == ChaseState && StateMachine.CurrentPawnState != DeadState)
            {
                Debug.Log("attack!");
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

    public void OnAttackFinished()
    {
        StateMachine.CurrentPawnState?.OnAttackFinished();
    }

    private IEnumerator FlashSprite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    } 

    public void Stunned()
    {
        GameObject stars = Instantiate(stunnedStarsPrefab, transform.position + new Vector3(0, 0.4f, 0), Quaternion.Euler(90, 0, 0), transform);
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

        StopAllCoroutines();
        IsAlive = false;
        OnEnemyKilled?.Invoke(this);
        animator.SetTrigger("death");
        StateMachine.ChangeState(DeadState);

        StartCoroutine(DestroyAfterTime(1f));
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}

