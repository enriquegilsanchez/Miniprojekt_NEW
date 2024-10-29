using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{

    Transform PlayerPos;

    GameObject Player;

    Component GameControl;
    float enemySpeed = 2.5f;
    Rigidbody2D rb;
    Vector2 movement;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        GameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent("GameControl");
        PlayerPos = Player.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 direction = PlayerPos.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
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


}
