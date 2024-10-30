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
    float moveSpeed = 3f;
    Rigidbody2D rb;
    Vector2 movement;

    [SerializeField]
    int Health;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        GameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent("GameControl");
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerPos = Player.transform.position;
        Vector3 direction = PlayerPos - transform.position;
        direction.Normalize();
        movement = direction;

    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed);
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameControl.SendMessage("GetHp");
            GameControl.SendMessage("ChangeHp", -1);
        }

    }

    public void ChangeHp(int val)
    {
        Health += val;
        if (Health <= 0)
        {
            // Dont need blood for skeletons
            // GameObject bloodeffect = Instantiate(Blood, transform.position, transform.rotation);
            // Destroy(bloodeffect.gameObject, 0.8f);
            GameControl.SendMessage("ChangeScore", 1);
            Destroy(gameObject);
        }
    }


}
