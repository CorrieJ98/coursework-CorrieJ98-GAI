using Unity.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using UnityEngine;

class AllyAgentFleeRocket : SteeringBehaviour
{
    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        return Vector3.zero;
    }
}

// should make a path for the rocket.

/*
take the forward vector of the rocket and extend this to "maxRocketRange + splashDamageRange + 1"
this will give the max dangerous range of the rocket + a small buffer distance.
agents will flee along an axis perpendicular to this vector
ie if rocket is flying on heading 180, flee 270 or 090 and make space for it until at a safe distance
 */