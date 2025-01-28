using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngageMeleeTask : Node
{
    private SkeletonBT enemy;

    public EngageMeleeTask(SkeletonBT enemy)
    {
        this.enemy = enemy; // Reference to the enemy
    }

    public override NodeState Evaluate()
    {
        if (enemy.health <= 0)
        {
            return NodeState.Failure; // Stop engaging if health is zero
        }

        // Stop movement
        enemy.rb.velocity = Vector2.zero;

        // Set animations
        enemy.animator.SetBool("isAttacking", true);
        enemy.animator.SetBool("isWalking", false);

        // Handle melee interaction with player
        enemy.time += Time.deltaTime;
        if (enemy.time >= enemy.iFrame)
        {
            enemy.time = 0f;

            Collider2D[] players = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.meleeRange);
            foreach (var player in players)
            {
                if (player.CompareTag("Player"))
                {
                    // Damage player and update game control
                    enemy.gameControl.SendMessage("GetHp");
                    player.GetComponent<PlayerController>().ChangeHp(-1); // Updated to use component
                }
            }
        }

        return NodeState.Running;
    }
}
