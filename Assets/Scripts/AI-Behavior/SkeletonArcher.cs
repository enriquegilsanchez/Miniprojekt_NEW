using UnityEngine;

public class SkeletonArcher1 : MonoBehaviour
{
    Vector3 PlayerPos;
    GameObject Player;
    Component GameControl;

    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float Speed = 3f; // Geschwindigkeit des Gegners
    Rigidbody2D rb;
    Vector2 movement;
    public EnemyWeapon weapon;

    public float shotTimer = 0f;
    public float shootingCooldown = 1f; // Zeit zwischen Schüssen
    public float detectionRange = 10f; // Reichweite für Angriff
    public float wakeUpRange = 5f; // Reichweite, in der der Gegner aufsteht

    [SerializeField]
    int health;

    public bool isAwake = false; // Gegner-Zustand (schlafend/aktiv)
    private Vector3 targetPoint;

    // Patrouillenpunkte
    public Transform pointA;
    public Transform pointB;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        GameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent("GameControl");
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

        // Position des Spielers
        PlayerPos = Player.transform.position;

        float distanceToPlayer = Vector3.Distance(PlayerPos, transform.position);

        if (!isAwake && distanceToPlayer <= wakeUpRange)
        {
            WakeUp();
        }

        if (isAwake)
        {
            if (distanceToPlayer <= detectionRange)
            {
                EngagePlayer();
            }
            else
            {
                Patrol();
            }
        }
    }

    void FixedUpdate()
    {
        if (health <= 0 || !isAwake)
        {
            return;
        }

        shotTimer += Time.deltaTime;

        // Spieler in Reichweite -> Angreifen
        if (Vector3.Distance(Player.transform.position, transform.position) <= detectionRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("speed", 0);
            animator.SetBool("isShooting", true);

            if (shotTimer >= shootingCooldown)
            {
                weapon.Shoot(); // Schießen
                shotTimer = 0f; // Timer zurücksetzen
            }
        }
    }

    void WakeUp()
    {
        isAwake = true;
        animator.SetTrigger("wakeUp"); // Wake-up-Animation starten
        Debug.Log("Skeleton Archer has awakened!");
    }

    void EngagePlayer()
    {
        // Spieler ist in Reichweite -> Animation einstellen
        Vector3 direction = PlayerPos - transform.position;
        direction.Normalize();
        spriteRenderer.flipX = direction.x <= 0;
        animator.SetBool("isShooting", true);
        animator.SetFloat("speed", 0);
    }

    void Patrol()
    {
        // Bewegung zwischen den Punkten
        transform.position = Vector2.MoveTowards(transform.position, targetPoint, Speed * Time.deltaTime);

        // Zielpunkt wechseln, wenn nah genug
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;
        }

        // Gegner-Ausrichtung anpassen
        Vector3 direction = targetPoint - transform.position;
        spriteRenderer.flipX = direction.x <= 0;

        // Animation einstellen
        animator.SetBool("isShooting", false);
        animator.SetFloat("speed", Speed);
    }

    public void ChangeHp(int val)
    {
        health += val;
        animator.SetFloat("hp", health);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        rb.velocity = Vector2.zero;
        animator.SetTrigger("die"); // Todesanimation
        Destroy(GetComponent<BoxCollider2D>());
        GameControl.SendMessage("ChangeScore", 1);
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wakeUpRange);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
