using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkeletonArcher : MonoBehaviour
{

    Vector3 PlayerPos;

    GameObject Player;

    Component GameControl;
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    float moveSpeed = 3f;
    Rigidbody2D rb;
    Vector2 movement;
    public EnemyWeapon weapon;


    private float shotTimer = 0f;

    [SerializeField]
    int health;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player");
        GameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent("GameControl");
        rb = GetComponent<Rigidbody2D>();
        animator.SetFloat("hp", health);
    }

    void Update()
    {
        PlayerPos = Player.transform.position;
        Vector3 direction = PlayerPos - transform.position;
        direction.Normalize();
        if (direction.x <= 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
        movement = direction;
    }
    void FixedUpdate()
    {
        shotTimer += Time.deltaTime;
        Debug.Log("ShotTimer" + shotTimer);
        if ((PlayerPos - transform.position).magnitude >= 5)
        {
            rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed);
            animator.SetFloat("speed", rb.velocity.magnitude);
        }
        else
        {
            if (shotTimer >= 5)
            {
                weapon.Shoot();
            }
        }
    }

    public void ChangeHp(int val)
    {
        health += val;
        animator.SetFloat("hp", health);
        if (health <= 0)
        {
            // Dont need blood for skeletons
            // GameObject bloodeffect = Instantiate(Blood, transform.position, transform.rotation);
            // Destroy(bloodeffect.gameObject, 0.8f);
            GameControl.SendMessage("ChangeScore", 1);
            Destroy(gameObject, 1.5f);
        }
    }


}
