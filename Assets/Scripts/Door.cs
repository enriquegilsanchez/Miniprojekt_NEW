using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject connectedTo;
    public string position;

    void Start() { }

    // Update is called once per frame
    void Update() { }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || connectedTo == null)
        {
            return;
        }
        Debug.Log("Entered door");

        if (position == "left")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x + 5,
                connectedTo.transform.position.y,
                connectedTo.transform.position.z
            );
        if (position == "right")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x - 5,
                connectedTo.transform.position.y,
                connectedTo.transform.position.z
            );
        if (position == "upper")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x,
                connectedTo.transform.position.y - 5,
                connectedTo.transform.position.z
            );
        if (position == "lower")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x,
                connectedTo.transform.position.y + 5,
                connectedTo.transform.position.z
            );
    }
}
