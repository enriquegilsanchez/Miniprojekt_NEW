using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMelee : Node
{
    private BossBT enemy;

    public BossMelee(BossBT enemy)
    {
        this.enemy = enemy; // Reference to the enemy
    }

    public override NodeState Evaluate()
    {
        if (enemy.health <= 0)
        {
            return NodeState.Failure; // Stop engaging if health is zero
        }

        // Check if melee attack is on cooldown
        if (enemy.meleeCooldown > 0)
        {
            return NodeState.Failure; // Melee attack is on cooldown
        }

        // Stop movement
        enemy.rb.velocity = Vector2.zero;

        // Set animations
        enemy.animator.SetBool("isWalking", false);
        enemy.animator.SetBool("isAttacking", true);

        // Perform melee attack
        Collider2D[] players = Physics2D.OverlapCircleAll(
            enemy.transform.position,
            enemy.meleeRange
        );
        foreach (var player in players)
        {
            if (player.CompareTag("Player"))
            {
                // Damage player and update game control
                enemy.gameControl.SendMessage("GetHp");
                player.GetComponent<PlayerController>().ChangeHp(-1);
            }
        }

        // Reset melee attack cooldown
        enemy.meleeCooldown = enemy.meleeCooldownDuration;

        return NodeState.Running;
    }
}
