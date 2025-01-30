using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatrol : Node
{
    private BossBT enemy;
    private Transform enemyTransform;
    private Transform playerTransform;

    public BossPatrol(BossBT enemy, Transform enemyTransform, Transform playerTransform)
    {
        this.enemy = enemy;
        this.enemyTransform = enemyTransform;
        this.playerTransform = playerTransform;
    }

    public override NodeState Evaluate()
    {
        if (enemy.isDead)
        {
            return NodeState.Failure;
        }

        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        if (distance <= enemy.lineOfSite)
        {
            return NodeState.Success;
        }

        Vector2 direction = (
            (Vector2)enemy.targetPoint - (Vector2)enemyTransform.position
        ).normalized;
        enemy.rb.velocity = direction * enemy.speed;

        // Check if the enemy reached the target point
        if (Vector2.Distance(enemyTransform.position, enemy.targetPoint) < 0.1f)
        {
            enemy.targetPoint = (enemy.targetPoint == enemy.pointA) ? enemy.pointB : enemy.pointA;
        }

        // Flip sprite based on direction
        enemy.FlipSprite(direction.x);

        // Update animations
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isAttacking", false);

        return NodeState.Running;
    }
}
