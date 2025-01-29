using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChase : Node
{
    private BossBT enemy;
    private Transform enemyTransform;
    private Transform playerTransform;

    public BossChase(BossBT enemy, Transform enemyTransform, Transform playerTransform)
    {
        this.enemy = enemy;
        this.enemyTransform = enemyTransform;
        this.playerTransform = playerTransform;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);

        if (
            enemy.health <= 0 /* || distance <= 3 */
        )
        {
            return NodeState.Failure;
        }
        if (distance <= 7)
        {
            return NodeState.Success;
        }

        // Calculate direction to the player
        Vector2 directionToPlayer = (
            (Vector2)playerTransform.position - (Vector2)enemyTransform.position
        ).normalized;
        enemy.rb.velocity = directionToPlayer * enemy.speed;

        // Flip sprite based on direction
        enemy.FlipSprite(directionToPlayer.x);

        // Update animations
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isRange", false);
        enemy.animator.SetBool("isAttacking", false);

        return NodeState.Running;
    }
}
