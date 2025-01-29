using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBT : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public Transform playerTransform;
    public Transform pointA;
    public Transform pointB;

    public float speed;
    public float lineOfSite;
    public float meleeRange;

    public Vector3 targetPoint;

    public int health = 100;
    public float iFrame = 1f;
    public float time = 0f;

    public Component gameControl;
    public bool isDead;

    private Node behaviorTree;

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

        // Construct Behavior Tree
        behaviorTree = new Selector(
            new List<Node>
            {
                new Sequence(
                    new List<Node>
                    {
                        new CheckPlayerInRange(transform, playerTransform, meleeRange),
                        new EngageMeleeTask(this),
                    }
                ),
                new Sequence(
                    new List<Node>
                    {
                        new CheckPlayerInRange(transform, playerTransform, lineOfSite),
                        new ChasePlayerTask(this),
                    }
                ),
                new PatrolSkeleton(this),
            }
        );
    }

    void FixedUpdate()
    {
        behaviorTree.Evaluate();
    }

    public void ChangeHp(int val)
    {
        health += val;
        animator.SetFloat("hp", health);
        if (health <= 0)
        {
            isDead = true;
            rb.velocity = Vector2.zero;
            Destroy(GetComponent<BoxCollider2D>());
            gameControl.SendMessage("ChangeScore", 1);
            Destroy(gameObject, 1f);
        }
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
}
