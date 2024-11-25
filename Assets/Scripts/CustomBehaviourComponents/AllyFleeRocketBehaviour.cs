using Unity.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using UnityEngine;

class AllyAgentFleeRocket : SteeringBehaviour
{
    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        // throw new System.NotImplementedException();

        return Vector3.zero;
    }
}

// should make a path for the rocket. think the prometheus school of running away but the opposite.

/*
take the forward vector of the rocket and extend this to "maxRocketRange + splashDamageRange + 1"
and then agents will flee along an axis perpendicular to this vector
ie if rocket is flying on heading 180, flee 270 or 090 and make space for it until at a safe distance

or i could just take the existing code from Hamid
 */