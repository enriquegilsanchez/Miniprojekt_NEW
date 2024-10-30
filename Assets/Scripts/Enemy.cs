using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    Transform PlayerPos;

    GameObject Player;

    Component GameControl;
    float enemySpeed = 2.5f;
    Rigidbody2D rb;
    Vector2 movement;

    [SerializeField]
    int Health;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        GameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent("GameControl");
        PlayerPos = Player.transform;
        rb = GetComponent<Rigidbody2D>();
        //rb.freezeRotation = true;
    }

    void Update()
    {
        Vector3 direction = PlayerPos.position - transform.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rb.rotation = angle;
        direction.Normalize();
        movement = direction;

    }
    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + (movement * enemySpeed * Time.deltaTime));
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
