using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBeam : MonoBehaviour
{
    public float duration = 2f; // Duration for which the beam stays active

    private void Start()
    {
        // Start the destruction timer
        StartCoroutine(DestroyBeamAfterDuration());
    }

    private IEnumerator DestroyBeamAfterDuration()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Destroy the beam game object
        Destroy(gameObject);
    }

    /* private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            other.GetComponent<PlayerController>().ChangeHp(-damage);

            // Optionally, destroy the beam immediately upon collision
            // Destroy(gameObject);
        }
    } */
}
