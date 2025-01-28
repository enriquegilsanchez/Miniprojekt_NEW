using UnityEngine;

public class enemy_arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    public float fireForce = 15f;

    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Player = GameObject.FindGameObjectWithTag("Player");
        UnityEngine.Vector3 direction = Player.transform.position - transform.position;
        UnityEngine.Vector3 rotation = transform.position - Player.transform.position;
        rb.velocity = new UnityEngine.Vector2(direction.x, direction.y).normalized * fireForce;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = UnityEngine.Quaternion.Euler(0, 0, rot + 180); // if it isnt horizontal add +90 to rot
    }

    // Update is called once per frame
    void Update() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Physics2D.IgnoreCollision(collision.collider, Ga);
        }
        Destroy(gameObject);
        if (collision.gameObject.CompareTag("Player"))
        {
            // Debug.Log("Arrow Collision");

            // Debug.Log("Player hit" + collision.gameObject.name);
            collision.gameObject.SendMessage("ChangeHp", -1);
        }
    }
}
