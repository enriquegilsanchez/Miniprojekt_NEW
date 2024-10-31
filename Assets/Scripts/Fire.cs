using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Blood;
    private UnityEngine.Vector3 mousePos;
    private Rigidbody2D rb;
    public float fireForce = 40f;
    void Start()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb = GetComponent<Rigidbody2D>();
        UnityEngine.Vector3 direction = mousePos - transform.position;
        UnityEngine.Vector3 rotation = transform.position - mousePos;
        rb.velocity = new UnityEngine.Vector2(direction.x, direction.y).normalized * fireForce;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = UnityEngine.Quaternion.Euler(0, 0, rot + 90);

    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player_Bullet"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<BoxCollider2D>());
            return;
        }
        GameObject Exp = Instantiate(Explosion, transform.position, transform.rotation);
        Destroy(Exp, 1f);
        Destroy(gameObject);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit" + collision.gameObject.name);
            collision.gameObject.SendMessage("ChangeHp", -1);
        }
    }
}
