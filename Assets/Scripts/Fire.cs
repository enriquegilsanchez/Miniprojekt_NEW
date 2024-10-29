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

    void OnCollisionEnter2D(Collision2D col) {

        GameObject Exp = Instantiate(Explosion, transform.position, transform.rotation);
        Destroy(Exp, 1f);
        Destroy(gameObject);

        if (col.gameObject.tag == "Zombie") {
            GameObject bloodeffect = Instantiate(Blood, col.transform.position, col.transform.rotation);
            Destroy(bloodeffect.gameObject, 0.8f);
            Destroy(col.gameObject);
            GameControl.Score++;
        }


    }
}
