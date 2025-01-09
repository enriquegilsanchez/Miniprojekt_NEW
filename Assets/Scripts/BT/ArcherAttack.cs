using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttack : Node
{
    private ArcherBT archer;

    public ArcherAttack(ArcherBT archer)
    {
        this.archer = archer;
    }

    public override NodeState Evaluate()
    {
        float distanceToPlayer = Vector3.Distance(archer.Player.transform.position, archer.transform.position);

        if (distanceToPlayer <= archer.detectionRange)
        {
            archer.rb.velocity = Vector2.zero;
            archer.animator.SetFloat("speed", 0);
            archer.animator.SetBool("isShooting", true);

            if (archer.shotTimer >= archer.shootingCooldown)
            {
                archer.weapon.Shoot();
                archer.shotTimer = 0f;
            }

            archer.shotTimer += Time.deltaTime;
            return NodeState.Running;
        }

        return NodeState.Failure;
    }
}
