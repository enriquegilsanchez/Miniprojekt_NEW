using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBT : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public Transform playerTransform;
    public GameObject beamPrefab;
    public Vector2 pointA;
    public Vector2 pointB;

    public float speed;
    public float lineOfSite;
    public float meleeRange;
    public float RangedRange;

    public Vector2 targetPoint;

    public int health = 100;
    public int maxHealth = 100;
    public float rangedCooldown = 0f; // Current cooldown time
    public float rangedCooldownDuration = 5f; // Total cooldown duration
    public float meleeCooldown = 0f; // Current cooldown time for melee attacks
    public float meleeCooldownDuration = 2f; // Total cooldown duration for melee attacks

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
        var currentRect = gameControl.GetComponent<GameControl>().currentRoom.rect;
        pointA = new Vector2(currentRect.xMin + 3, currentRect.center.y);
        pointA = new Vector2(currentRect.xMax - 3, currentRect.center.y);

        // Start patrol point
        targetPoint = pointA;

        // Construct Behavior Tree
        behaviorTree = new Selector(
            new List<Node>
            {
                new Sequence(
                    new List<Node>
                    {
                        new CheckPlayerInRange(transform, playerTransform, meleeRange),
                        new BossMelee(this),
                    }
                ),
                new Sequence(
                    new List<Node>
                    {
                        new BossCheckHealth(this, 0.5f), // Check if health is below 50%
                        new CheckPlayerInRange(transform, playerTransform, RangedRange),
                        new BossRanged(this), // Perform the ranged attack
                    }
                ),
                new Sequence(
                    new List<Node>
                    {
                        new CheckPlayerInRange(transform, playerTransform, lineOfSite),
                        new BossChase(this, this.transform, playerTransform),
                    }
                ),
                new BossPatrol(this, this.transform, playerTransform),
            }
        );
    }

    private void Update()
    {
        // Reduce cooldown over time
        if (rangedCooldown > 0)
        {
            rangedCooldown -= Time.deltaTime;
        }
        if (meleeCooldown > 0)
        {
            meleeCooldown -= Time.deltaTime;
        }
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
            animator.SetTrigger("Dead");
            rb.velocity = Vector2.zero;
            Destroy(GetComponent<BoxCollider2D>());
            gameControl.SendMessage("ChangeScore", 1);
            /* Destroy(gameObject, 1f); */
        }
    }

    /* void OnCollisionStay2D(Collision2D collision)
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
    } */

    /* void OnCollisionEnter2D(Collision2D collision)
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
    } */
    void Die()
    {
        /* animator.SetTrigger("Death"); */
        Destroy(
            gameObject /* , 2f */
        ); // Gegner wird nach 2 Sekunden entfernt
    }

    public void FlipSprite(float directionX)
    {
        if (directionX != 0)
        {
            spriteRenderer.flipX = directionX > 0;
        }
    }

    public void ApplyMeleeDamage()
    {
        Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, meleeRange);
        foreach (var player in players)
        {
            if (player.CompareTag("Player"))
            {
                gameControl.SendMessage("GetHp");
                player.GetComponent<PlayerController>().ChangeHp(-1);
            }
        }
    }

    public void Destroy()
    {
        Destroy(
            gameObject /* , 2f */
        ); // Gegner wird nach 2 Sekunden entfernt
    }
}
