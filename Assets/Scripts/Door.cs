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
        // Teleport the player to the next room with clearance depending on which direction the door was facing
        if (position == "left")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x - 2,
                connectedTo.transform.position.y,
                connectedTo.transform.position.z
            );
        if (position == "right")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x + 2,
                connectedTo.transform.position.y,
                connectedTo.transform.position.z
            );
        if (position == "upper")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x,
                connectedTo.transform.position.y + 2,
                connectedTo.transform.position.z
            );
        if (position == "lower")
            collision.gameObject.transform.position = new Vector3(
                connectedTo.transform.position.x,
                connectedTo.transform.position.y - 2,
                connectedTo.transform.position.z
            );
    }
}
