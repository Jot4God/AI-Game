using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Movimento")]
    public Transform player;
    public float speed = 4f;
    public float chaseDistance = 8f;
    public float attackDistance = 1.5f;
    public Transform[] patrolPoints;
    public float patrolWait = 2f;

    [Header("Vida")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Ataque")]
    public int damage = 30;            // Dano que este inimigo causa
    public float attackCooldown = 1f;  // Tempo entre ataques

    [Header("Extras")]
    public int xpReward = 20;
    public GameObject healthPickupPrefab;
    public GameObject moneyPickupPrefab;
    public int moneyDropAmount = 1;

    // Feedback de dano
    private SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;
    private Color originalColor;

    private int currentPatrol = 0;
    private float waitTimer = 0f;
    private Rigidbody rb;
    private Vector3 currentDirection = Vector3.zero;

    private float lastAttackTime = 0f;

    // ======================
    // IA: Decision Tree + FSM
    // ======================
    private EnemyDecisionTree decisionTree;
    private EnemyState currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        decisionTree = new EnemyDecisionTree();

        // Estado inicial: patrulhar
        currentState = new EnemyPatrolState(this);
        currentState.Enter();
    }

    void Update()
    {
        if (player == null) return;

        // DECISION TREE
        float dist = Vector3.Distance(transform.position, player.position);
        float healthRatio = (float)currentHealth / Mathf.Max(1, maxHealth);

        var decision = decisionTree.Evaluate(dist, healthRatio, chaseDistance, attackDistance);

        // FSM – escolher estado
        EnemyState newState = currentState;

        switch (decision)
        {
            case EnemyDecisionTree.Decision.Patrol:
                if (!(currentState is EnemyPatrolState))
                    newState = new EnemyPatrolState(this);
                break;

            case EnemyDecisionTree.Decision.Chase:
                if (!(currentState is EnemyChaseState))
                    newState = new EnemyChaseState(this);
                break;

            case EnemyDecisionTree.Decision.Attack:
                if (!(currentState is EnemyAttackState))
                    newState = new EnemyAttackState(this);
                break;

            case EnemyDecisionTree.Decision.Flee:
                if (!(currentState is EnemyFleeState))
                    newState = new EnemyFleeState(this);
                break;
        }

        if (newState != currentState)
        {
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }

        currentState.Update();

        // DEBUG: teste manual de dano
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(10);
    }

    void FixedUpdate()
    {
        if (currentDirection != Vector3.zero)
        {
            Vector3 move = currentDirection * speed * Time.fixedDeltaTime;
            Vector3 newPos = transform.position + move;
            rb.MovePosition(newPos);
        }
    }

    // ======================
    // Métodos usados pelos Estados (FSM)
    // ======================

    public void DoPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrol];
        if (targetPoint == null)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 targetPos = targetPoint.position;
        float distToPoint = Vector3.Distance(transform.position, targetPos);

        if (distToPoint < 0.3f)
        {
            currentDirection = Vector3.zero;
            waitTimer += Time.deltaTime;

            if (waitTimer >= patrolWait)
            {
                currentPatrol = (currentPatrol + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }
            return;
        }

        // Path-Finding (A*) se existir
        if (AStarGrid.Instance != null)
        {
            Vector3 nextStep = AStarGrid.Instance.GetNextStep(transform.position, targetPos);
            Vector3 dir = nextStep - transform.position;
            dir.y = 0f; // se usares XZ; se usares XY, troca para dir.z = 0f;
            currentDirection = dir.normalized;
        }
        else
        {
            currentDirection = (targetPos - transform.position).normalized;
        }
    }

    public void DoChase()
    {
        if (player == null)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 targetPos = player.position;

        if (AStarGrid.Instance != null)
        {
            Vector3 nextStep = AStarGrid.Instance.GetNextStep(transform.position, targetPos);
            Vector3 dir = nextStep - transform.position;
            dir.y = 0f;
            currentDirection = dir.normalized;
        }
        else
        {
            currentDirection = (targetPos - transform.position).normalized;
        }
    }

    public void DoAttack()
    {
        currentDirection = Vector3.zero;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (player != null)
            {
                PlayerHP ph = player.GetComponent<PlayerHP>();
                if (ph != null)
                {
                    ph.TakeDamage(damage);
                }
            }

            lastAttackTime = Time.time;
        }
    }

    public void DoFlee()
    {
        if (player == null)
        {
            currentDirection = Vector3.zero;
            return;
        }

        Vector3 dir = transform.position - player.position;
        dir.y = 0f;
        currentDirection = dir.normalized;
    }

    public void SetDirection(Vector3 dir)
    {
        currentDirection = dir;
    }

    // ======================
    // Dano e morte (igual ao teu)
    // ======================

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(name + " recebeu " + damage + " de dano! Vida atual: " + currentHealth);

        if (spriteRenderer != null)
            StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer == null)
            yield break;

        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log(name + " morreu!");

        if (player != null)
        {
            PlayerLevel playerLevel = player.GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddXP(xpReward);
                Debug.Log("Jogador ganhou " + xpReward + " XP!");
            }
        }

        if (healthPickupPrefab != null && Random.value < 0.5f)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, -5f, 0f);
            Instantiate(healthPickupPrefab, spawnPos, Quaternion.identity);
        }

        if (moneyPickupPrefab != null && Random.value < 0.7f)
        {
            for (int i = 0; i < moneyDropAmount; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(0f, -5f, 0f);
                Instantiate(moneyPickupPrefab, spawnPos, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
