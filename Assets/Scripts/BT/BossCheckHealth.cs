using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCheckHealth : Node
{
    private BossBT enemy;
    private float healthThreshold;

    public BossCheckHealth(BossBT enemy, float healthThreshold)
    {
        this.enemy = enemy;
        this.healthThreshold = healthThreshold;
    }

    public override NodeState Evaluate()
    {
        return enemy.health <= enemy.maxHealth * healthThreshold
            ? NodeState.Success
            : NodeState.Failure;
    }
}
