using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public int health = 5;

    private Animator animator;
    private bool movingToPointB = true;
    private bool facingRight = true;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (health <= 0) return; // Gegner ist tot, nichts mehr machen

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Angriff starten
            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack");
                Invoke("DamagePlayer", 0.5f); // Spieler nach einer kurzen Verzögerung angreifen
            }
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Spieler verfolgen
            animator.SetBool("isWalking", true);
            MoveTowards(player.position);
        }
        else
        {
            // Zwischen Punkt A und B laufen
            animator.SetBool("isWalking", true);
            Patrol();
        }
    }

    void Patrol()
    {
        if (movingToPointB)
        {
            MoveTowards(pointB.position);

            if (Vector2.Distance(transform.position, pointB.position) < 0.1f)
                movingToPointB = false;
        }
        else
        {
            MoveTowards(pointA.position);

            if (Vector2.Distance(transform.position, pointA.position) < 0.1f)
                movingToPointB = true;
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (direction.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            animator.SetTrigger("Death");
            /* Die(); */
        }
    }

    void Die()
    {
        /* animator.SetTrigger("Death"); */
        Destroy(gameObject/* , 2f */); // Gegner wird nach 2 Sekunden entfernt
    }

    void DamagePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ChangeHp(-1); // Spieler verliert 1 Gesundheit
            }
        }
        isAttacking = false; // Angriff beenden
    }
}
