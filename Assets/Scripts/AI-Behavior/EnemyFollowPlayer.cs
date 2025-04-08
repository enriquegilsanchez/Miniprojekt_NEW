/*using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    Vector3 playerPos;
    GameObject player;
    Component gameControl;

    public Animator animator;
    private SpriteRenderer spriteRenderer;
   // float moveSpeed = 3f;
    Rigidbody2D rb;
    Vector2 movement;

    private float iFrame = 1f;
    private float time = 0f;

    [SerializeField]
    int health;

    public float speed;
    public float lineOfSite;
    private Transform playerTransform;

    // Punkte für die Patrouille
    public Transform pointA;
    public Transform pointB;
    private Vector3 targetPoint;

    private bool isChasing = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent("GameControl");
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("hp", health);

        // Startpunkt der Patrouille
        targetPoint = pointA.position;
    }

    void Update()
    {
        if (health <= 0)
        {
            return;
        }

        // Abstand zum Spieler
        float distanceFromPlayer = Vector2.Distance(playerTransform.position, transform.position);

        if (distanceFromPlayer < lineOfSite)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            // Verfolgung des Spielers
            ChasePlayer();
        }
        else
        {
            // Patrouille zwischen den Punkten
            Patrol();
        }
    }

    void Patrol()
    {
        // Bewege dich in Richtung des aktuellen Zielpunkts
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Wechsel den Zielpunkt, wenn der Gegner nahe genug ist
        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;
        }

        // Animation und Ausrichtung
        Vector3 direction = targetPoint - transform.position;
        direction.Normalize();
        spriteRenderer.flipX = direction.x <= 0;
    }

    void ChasePlayer()
    {
        // Bewege dich in Richtung des Spielers
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);

        // Animation und Ausrichtung
        Vector3 direction = playerTransform.position - transform.position;
        direction.Normalize();
        spriteRenderer.flipX = direction.x <= 0;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (health <= 0)
        {
            return;
        }
        time += Time.deltaTime;
        if (time >= iFrame)
        {
            time = 0f;
            if (collision.gameObject.CompareTag("Player"))
            {
                gameControl.SendMessage("GetHp");
                playerTransform.SendMessage("ChangeHp", -1);
            }
        }
    }
     void OnCollisionEnter2D(Collision2D collision)
    {
        if (health <= 0)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            gameControl.SendMessage("GetHp");
            playerTransform.SendMessage("ChangeHp", -1);
        }
    }



    public void ChangeHp(int val)
    {
        health += val;
        animator.SetFloat("hp", health);
        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            Destroy(GetComponent<BoxCollider2D>());
            gameControl.SendMessage("ChangeScore", 1);
            Destroy(gameObject, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
*/
using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
    Vector3 playerPos;
    GameObject player;
    Component gameControl;

    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private float iFrame = 1f;
    private float time = 0f;

    [SerializeField]
    int health;

    public float speed;
    public float lineOfSite;
    public float meleeRange; // Melee range
    private Transform playerTransform;

    // Patrol points
    public Transform pointA;
    public Transform pointB;
    private Vector3 targetPoint;

    private bool isChasing = false;
    private bool isInMeleeRange = false;

    // Separation settings
    public float separationRadius = 1f; // Radius for detecting nearby enemies
    public float separationForce = 2f; // Strength of the repulsion force
    public LayerMask enemyLayer; // Layer for enemy detection

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        gameControl = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent("GameControl");
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("hp", health);

        // Start patrol point
        targetPoint = pointA.position;
    }

    void FixedUpdate()
    {
        if (health <= 0)
        {
            return;
        }

        // Distance to player
        float distanceFromPlayer = Vector2.Distance(playerTransform.position, transform.position);

        // Check melee range
        isInMeleeRange = distanceFromPlayer <= meleeRange;

        if (isInMeleeRange)
        {
            EngageInMelee();
        }
        else if (distanceFromPlayer < lineOfSite)
        {
            isChasing = true;
            ChasePlayerWithSeparation();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }

    void Patrol()
    {
        // Move towards the current target point
        Vector2 direction = (targetPoint - transform.position).normalized;
        rb.velocity = direction * speed;

        // Switch target point if close enough
        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;
        }

        // Set sprite orientation and animation
        spriteRenderer.flipX = direction.x <= 0;
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void ChasePlayerWithSeparation()
    {
        // Base direction towards the player
        Vector2 directionToPlayer = (
            (Vector2)playerTransform.position - (Vector2)transform.position
        ).normalized;

        // Apply separation force
        Vector2 separation = CalculateSeparation();

        // Combine chasing direction with separation force
        Vector2 finalDirection = (directionToPlayer + separation).normalized;

        // Move the enemy using Rigidbody
        rb.velocity = finalDirection * speed;

        // Set sprite orientation and animation
        spriteRenderer.flipX = finalDirection.x <= 0;
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void EngageInMelee()
    {
        // Stop moving and attack in melee range
        rb.velocity = Vector2.zero;

        // Set animations
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);

        // Set sprite orientation
        Vector3 direction = playerTransform.position - transform.position;
        spriteRenderer.flipX = direction.x <= 0;
    }

    Vector2 CalculateSeparation()
    {
        // Find nearby enemies
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius,
            enemyLayer
        );
        Vector2 separationForce = Vector2.zero;

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject) // Exclude self
            {
                Vector2 diff = (Vector2)transform.position - (Vector2)enemy.transform.position;
                separationForce += diff.normalized / diff.magnitude; // Weighted by distance
            }
        }

        return separationForce * separationForce; // Scale by separation strength
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (health <= 0)
        {
            return;
        }
        time += Time.deltaTime;
        if (time >= iFrame)
        {
            time = 0f;
            if (collision.gameObject.CompareTag("Player"))
            {
                gameControl.SendMessage("GetHp");
                playerTransform.SendMessage("ChangeHp", -1);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (health <= 0)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            gameControl.SendMessage("GetHp");
            playerTransform.SendMessage("ChangeHp", -1);
        }
    }

    public void ChangeHp(int val)
    {
        health += val;
        animator.SetFloat("hp", health);
        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            Destroy(GetComponent<BoxCollider2D>());
            gameControl.SendMessage("ChangeScore", 1);
            Destroy(gameObject, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // Separation visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
