using Unity.Collections;
using System.Collections.Generic;
using System.Buffers.Text;
using UnityEngine;

using UnityEngine;

public class AllyAgentWander : SteeringBehaviour
{
    [SerializeField] protected float circleRadius = 400.0f;
    [SerializeField] protected float circleDistance = 500.0f;
    [SerializeField] protected float maxRandomDisplacement = 30.0f;

    private Vector3 previousTargetPosition;

    protected override void Start()
    {
        base.Start();

        transform.up = RandomPointOnUnitCircleCircumference();

        previousTargetPosition = transform.up * (circleDistance + circleRadius);
    }

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        // First get a random position on the circumference of a circle which can be used as a direction
        Vector3 targetPosition = RandomPointOnUnitCircleCircumference();

        // Scale the direction by the maximum amount of displacement to get the "small circle"
        targetPosition *= maxRandomDisplacement;

        // Add the previous target position to get a displacement from the last target position
        targetPosition += previousTargetPosition;

        // These 2 lines effectly remap the target position to a point on the circumference of the imaginary circle
        // Get the centre position of the imaginary circle and then calculate the direction to the target position
        // Normalise to a unit vector so it can be scaled to the size of the imaginary circle and then add on
        // the position of the circle to get the point back on to the circumference of the circle
        Vector3 circlePosition = transform.position + (transform.up * circleDistance);
        targetPosition = circlePosition + (Vector3.Normalize(targetPosition - circlePosition) * circleRadius);

        // Update the previous target position with the new position
        previousTargetPosition = targetPosition;

        // Get the desired velocity just like seek and limit to maxSpeed
        desiredVelocity = Vector3.Normalize(targetPosition - transform.position) * SteeringAgent.MaxCurrentSpeed;

        // Calculate steering velocity
        steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
        return steeringVelocity;
    }

    protected static Vector3 RandomPointOnUnitCircleCircumference()
    {
        float randomAngle = Random.value * (2.0f * Mathf.PI);
        return new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0.0f);

        // Could also use Unity and do the following:
        //Vector3 circumferencePosition = Random.onUnitSphere;
        //circumferencePosition.z = 0.0f;
        //circumferencePosition.Normalize();
        //return circumferencePosition;
    }

    public override void DebugDraw(SteeringAgent steeringAgent)
    {
        Vector3 circlePosition = transform.position + (transform.up * circleDistance);
        DebugDrawCircle("DebugCircle " + GetType().Name, circlePosition, circleRadius, Color.magenta);

        base.DebugDraw(steeringAgent);
    }
}
