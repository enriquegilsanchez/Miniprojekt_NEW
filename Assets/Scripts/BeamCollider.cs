using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCollider : MonoBehaviour
{
    public int damage = 2;   // Damage dealt to the player
    private bool hasDamaged = false; // Flag to ensure damage is applied only once

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDamaged) return; // Skip if damage has already been applied

        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            other.GetComponent<PlayerController>().ChangeHp(-damage);
            
            // Set the flag to true so no further damage is applied
            hasDamaged = true;

            // Disable the collider to prevent further triggers
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
