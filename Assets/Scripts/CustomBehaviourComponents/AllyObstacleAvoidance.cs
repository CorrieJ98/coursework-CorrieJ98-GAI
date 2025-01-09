using Unity.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using UnityEngine;

class AllyObstacleAvoidance : SteeringBehaviour
{ 
    private Vector2Int[] directions = new Vector2Int[]
    {
        
        Vector2Int.up,           // up
        Vector2Int.one,          // up right
        Vector2Int.right,        // right
        new Vector2Int(1,-1),    // down right 
        Vector2Int.down,         // down
        -Vector2Int.one,         // down left
        Vector2Int.left,         // left
        new Vector2Int(-1,1)     // up left
    };

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        var allySteeringAgent = steeringAgent as AllyAgent;
        Vector3[] steeringQueues = new Vector3[7];
        float scalar = 1.0f;
        int i = 0;

        foreach (var dir in directions) {

            if (!GameData.Instance.Map.IsNavigatable((int)(dir.x + transform.position.x), (int)(dir.y + transform.position.y)))
            {
                Vector3 targetPos = new Vector3();

                targetPos.x = (int)(dir.x + transform.position.x);
                targetPos.y = (int)(dir.y + transform.position.y);
                targetPos.z = 0;

                desiredVelocity = Vector3.Normalize(transform.position - targetPos) * SteeringAgent.MaxCurrentSpeed;

                steeringQueues[i] = desiredVelocity.normalized;
                ++i;
                ++scalar;
            }
        }  

        foreach(var s in steeringQueues)
        {
            desiredVelocity += s;
        }

        steeringVelocity =  allySteeringAgent.CurrentVelocity - desiredVelocity;

        return steeringVelocity;
    }
}