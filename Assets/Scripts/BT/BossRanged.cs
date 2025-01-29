using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRanged : Node
{
    private BossBT enemy;
    private int numberOfBeams = 8; // Number of beams to spawn
    private float radius = 10f; // Radius around the player's last position
    private float beamDelay = 0.2f; // Delay between each beam

    public BossRanged(BossBT enemy)
    {
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if (enemy.health <= 0)
        {
            return NodeState.Failure; // Boss is dead
        }

        // Check if the ranged attack is on cooldown
        if (enemy.rangedCooldown > 0)
        {
            return NodeState.Failure; // Ranged attack is on cooldown
        }

        // Stop movement
        enemy.rb.velocity = Vector2.zero;

        // Set animation for ranged attack
        enemy.animator.SetBool("isWalking", false);
        enemy.animator.SetBool("isRange", true);

        // Start the coroutine to spawn beams
        enemy.StartCoroutine(SpawnBeamsCoroutine(enemy.playerTransform.position));

        // Reset ranged attack cooldown
        enemy.rangedCooldown = enemy.rangedCooldownDuration;

        return NodeState.Running;
    }

    private IEnumerator SpawnBeamsCoroutine(Vector3 playerLastPosition)
    {
        for (int i = 0; i < numberOfBeams; i++)
        {
            // Generate random angle and distance within the circle
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Random angle in radians
            float randomDistance = Random.Range(0f, radius); // Random distance within the radius

            // Calculate beam position
            Vector3 beamPosition =
                playerLastPosition
                + new Vector3(
                    Mathf.Cos(randomAngle) * randomDistance,
                    Mathf.Sin(randomAngle) * randomDistance,
                    0
                );

            // Spawn the beam
            GameObject beam = Object.Instantiate(
                enemy.beamPrefab,
                beamPosition,
                Quaternion.identity
            );

            // Ensure the beam remains vertical
            beam.transform.rotation = Quaternion.identity; // Reset rotation to default (vertical)

            // Wait before spawning the next beam
            yield return new WaitForSeconds(beamDelay);
        }
    }
}
