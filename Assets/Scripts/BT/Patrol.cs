using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTask : Node
{

    private ArcherBT enemy;
    public PatrolTask(ArcherBT enemy)
    {
        this.enemy = enemy;
    }

    public override NodeState Evaluate()
    {
        if (enemy.isDead)
        {
            return NodeState.Failure;
        }
        Vector2 direction = ((Vector2)enemy.targetPoint - (Vector2)enemy.transform.position).normalized;
        enemy.rb.velocity = direction * enemy.speed;
        if (Vector2.Distance(enemy.transform.position, enemy.targetPoint) < 0.1f)
        {
            enemy.targetPoint = (enemy.targetPoint == enemy.pointA.position) ? enemy.pointB.position : enemy.pointA.position;
        }

        enemy.spriteRenderer.flipX = direction.x <= 0;
        enemy.animator.SetBool("isWalking", true);
        enemy.animator.SetBool("isAttacking", false);

        return NodeState.Running;
    }
}
