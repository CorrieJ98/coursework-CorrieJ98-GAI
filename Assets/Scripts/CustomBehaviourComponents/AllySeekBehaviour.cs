/*
 Taken and adapted from Hamid's EnemySeekBehaviour.cs
 */

using Unity.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using UnityEngine;

class AllyAgentSeek : SteeringBehaviour
{

    [SerializeField] private float arrivalRadius = 100f;
    public override Vector3 UpdateBehaviour(SteeringAgent _steeringAgent)
    {
        
        var allySteeringAgent = _steeringAgent as AllyAgent;
        
        Vector3 targetPosition = allySteeringAgent.startPosition;
        Vector3 moveDist = targetPosition - transform.position;

        var nearestEnemy = SteeringAgent.GetNearestAgent(transform.position, GameData.Instance.enemies);
        if (nearestEnemy != null) //&&
        /*    (targetPosition - transform.position).magnitude < 10.0f &&
        (nearestEnemy.transform.position - transform.position).magnitude <= Attack.AllyGunData.range) */{
            targetPosition = nearestEnemy.transform.position;
        }

        if (moveDist.magnitude <= (arrivalRadius))
        {
            Arrival(moveDist);
        }
            desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * SteeringAgent.MaxCurrentSpeed;
        steeringVelocity = desiredVelocity - _steeringAgent.CurrentVelocity;
        return steeringVelocity;
    }

    private Vector3 Arrival(Vector3 _moveDist)
    {   
        return desiredVelocity = Vector3.Normalize(_moveDist) * _moveDist.magnitude;
    }
}