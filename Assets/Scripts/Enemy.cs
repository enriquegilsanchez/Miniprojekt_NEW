using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Vector3 PlayerPos;

    GameObject Player;

    Component GameControl;
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    float moveSpeed = 3f;
    Rigidbody2D rb;
    Vector2 movement;

    private float iFrame = 1f;
    private float time = 0f;

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
        rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed);
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        time += Time.deltaTime;
        if (time >= iFrame)
        {
            time = 0f;
            if (collision.gameObject.CompareTag("Player"))
            {
                GameControl.SendMessage("GetHp");
                Player.SendMessage("ChangeHp", -1);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameControl.SendMessage("GetHp");
            Player.SendMessage("ChangeHp", -1);
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
            Destroy(gameObject);
        }
    }


}
