using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUpTask : Node
{
    private ArcherBT archer;

    public WakeUpTask(ArcherBT archer)
    {
        this.archer = archer;
    }

    public override NodeState Evaluate()
    {
        float distanceToPlayer = Vector3.Distance(
            archer.Player.transform.position,
            archer.transform.position
        );

        if (!archer.isAwake && distanceToPlayer <= archer.wakeUpRange)
        {
            archer.WakeUp();
            return NodeState.Success;
        }

        return archer.isAwake ? NodeState.Failure : NodeState.Running;
    }
}
