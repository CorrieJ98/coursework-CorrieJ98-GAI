using Unity.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using UnityEngine;

class AllyAgentFlee : SteeringBehaviour
{
    // taken from AllySeekBehaviour.cs with the vector flipped
    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        var allySteeringAgent = steeringAgent as AllyAgent;

        Vector3 targetPosition = allySteeringAgent.startPosition;

        var nearestAlly = SteeringAgent.GetNearestAgent(transform.position, GameData.Instance.allies);
        if (nearestAlly != null &&
            (targetPosition - transform.position).magnitude < 10.0f &&
            (nearestAlly.transform.position - transform.position).magnitude <= Attack.AllyGunData.range)
        {
            targetPosition = nearestAlly.transform.position;
        }

        desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * SteeringAgent.MaxCurrentSpeed;
        steeringVelocity = steeringAgent.CurrentVelocity - desiredVelocity; // vector flipped here
        return steeringVelocity;
    }
}