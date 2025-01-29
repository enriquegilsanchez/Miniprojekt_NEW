using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBT : MonoBehaviour
{
    public GameObject Player;
    public Component GameControl;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public EnemyWeapon weapon;

    public Transform pointA;
    public Transform pointB;

    public float speed = 3f;
    public float shootingCooldown = 1f;
    public float detectionRange = 10f;
    public float wakeUpRange = 5f;

    [SerializeField]
    public int health;

    public bool isAwake = false;

    public float shotTimer = 0f;
    public Vector3 targetPoint;

    private Node behaviorTree;
    public bool isDead;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        GameControl = GameObject
            .FindGameObjectWithTag("GameController")
            .GetComponent("GameControl");
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("hp", health);

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
        GameControl.SendMessage("ChangeScore", 1);
        Destroy(gameObject, 1f);
    }
}
