using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Blood;
    void Start()
    {
        Destroy(gameObject, 2.0f);
    }

    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
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
