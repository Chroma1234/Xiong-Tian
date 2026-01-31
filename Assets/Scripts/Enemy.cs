using System;
using System.Collections;
using System.Xml;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public static event Action<Enemy> OnEnemyHit;
    public static event Action<Enemy> OnEnemyKilled;

    SpriteRenderer spriteRenderer;

    [SerializeField] private int maxHealth = 100;
    [HideInInspector] private int health;

    [SerializeField] public float knockbackForce = 8f;

    [SerializeField] private float hitFlashDuration;

    [SerializeField] private bool moveTest;

    //raptor-x-z
    public GameObject player;
    public Vector3 leftLimit;
    public Vector3 rightLimit;
    public SpriteRenderer sprite;

    #region States
    public PawnStateMachine StateMachine { get; set; }
    public PawnIdleState IdleState { get; set; }
    public PawnChaseState ChaseState { get; set; }
    #endregion

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);

            if (health <= 0 && IsAlive)
            {
                //death logic
                StopAllCoroutines();
                IsAlive = false;
                OnEnemyKilled?.Invoke(this);
                Destroy(gameObject);
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

        // Enter hit state (unless dead)
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

        //raptor-x-z
        //Getting the Positions of the pathfinding Limits
        leftLimit = transform.GetChild(2).gameObject.transform.position;
        rightLimit = transform.GetChild(3).gameObject.transform.position;

        sprite = this.GetComponent<SpriteRenderer>();

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
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentPawnState.PhysicsUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject obj = collision.gameObject;
      
        if(obj.layer == LayerMask.NameToLayer("Player") && StateMachine.CurrentPawnState != ChaseState)
        {
            player = obj;
            StateMachine.ChangeState(ChaseState);
        }
    }

    private IEnumerator FlashSprite()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
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
}

