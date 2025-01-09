using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPlayerInRange : Node
{
    private Transform enemyTransform;
    private Transform playerTransform;
    private float range;

    public CheckPlayerInRange(Transform enemy, Transform player, float range)
    {
        this.enemyTransform = enemy;
        this.playerTransform = player;
        this.range = range;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector2.Distance(enemyTransform.position, playerTransform.position);
        return distance <= range ? NodeState.Success : NodeState.Failure;
    }
}
