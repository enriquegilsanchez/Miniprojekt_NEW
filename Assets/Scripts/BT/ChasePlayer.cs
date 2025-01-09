using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerTask : Node
{
    private SkeletonBT enemy;

    public ChasePlayerTask(SkeletonBT enemy)
    {
        this.enemy = enemy; // Reference to the enemy
    }

    public override NodeState Evaluate()
    {
        if (enemy.health <= 0)
        {
            return NodeState.Failure; // Stop chasing if health is zero
        }

        Vector2 directionToPlayer = ((Vector2)enemy.playerTransform.position - (Vector2)enemy.transform.position).normalized;
        enemy.rb.velocity = directionToPlayer * enemy.speed;

        enemy.spriteRenderer.flipX = directionToPlayer.x <= 0;
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isAttacking", false);

        return NodeState.Running;
    }
}
