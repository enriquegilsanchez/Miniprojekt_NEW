using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBT : MonoBehaviour
{
    public GameObject Player;
    public Component gameControl;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public EnemyWeapon weapon;

    public Vector2 pointA;
    public Vector2 pointB;

    public float speed = 3f;
    public float shootingCooldown = 1f;
    public float detectionRange = 10f;
    public float wakeUpRange = 5f;

    [SerializeField]
    public int health;

    public bool isAwake = false;

    public float shotTimer = 0f;
    public Vector2 targetPoint;

    private Node behaviorTree;
    public bool isDead;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        gameControl = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent("GameControl");
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("hp", health);
        // Set random points to patrol
        var currentRect = gameControl.GetComponent<GameControl>().currentRoom.rect;
        var randomX = Random.Range(currentRect.xMin + 3, currentRect.xMax - 3);
        var randomY = Random.Range(currentRect.yMin + 3, currentRect.yMax - 3);
        pointA = new Vector2(randomX, randomY);
        randomX = Random.Range(currentRect.xMin + 3, currentRect.xMax - 3);
        randomY = Random.Range(currentRect.yMin + 3, currentRect.yMax - 3);
        pointA = new Vector2(randomX, randomY);

        behaviorTree = new Selector(
            new List<Node>
            {
                new WakeUpTask(this),
                new ArcherAttack(this),
                /* new Sequence(new List<Node>
                {
                    new ArcherAttack(this),
                    new PatrolTask(this)
                }), */
                new PatrolTask(this),
            }
        );
    }

    void FixedUpdate()
    {
        if (health > 0)
        {
            behaviorTree.Evaluate();
        }
    }

    public void WakeUp()
    {
        isAwake = true;
        animator.SetTrigger("wakeUp");
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
        animator.SetTrigger("die");
        Destroy(GetComponent<BoxCollider2D>());
        gameControl.SendMessage("ChangeScore", 1);
        Destroy(gameObject, 1f);
    }
}
