using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class AllyAgent : SteeringAgent
{
	private Pathfinder allyPathfinder;

	private AllyAgentSeek seekBehaviour;
	private AllyAgentWander wanderBehaviour;
	private AllyObstacleAvoidance avoidBehaviour;


	public SteeringAgent nearestAlly;
	public SteeringAgent nearestEnemy;
	public Vector3 rocketPosition;
	public Vector3 startPosition;

	private Vector3 targetPosition;
	private int pathIterator;
	private Attack.AttackType attackType = Attack.AttackType.AllyGun;

	protected override void InitialiseFromAwake()
	{
		allyPathfinder = gameObject.AddComponent<Pathfinder>();
		seekBehaviour = gameObject.AddComponent<AllyAgentSeek>();
		wanderBehaviour = gameObject.AddComponent<AllyAgentWander>();
		avoidBehaviour = gameObject.AddComponent<AllyObstacleAvoidance>();
	}

	protected override void CooperativeArbitration()
	{
		List<Node> currentPath;

		base.CooperativeArbitration();

		nearestEnemy = SteeringAgent.GetNearestAgent(transform.position, GameData.Instance.enemies);

		if (nearestEnemy != null)
		{
			currentPath = allyPathfinder.ExecuteAlgorithm(transform.position, nearestEnemy.transform.position);
		}

		attackType = Attack.AttackType.Melee;

		#region Keybindings
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			attackType = Attack.AttackType.Melee;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			attackType = Attack.AttackType.AllyGun;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			attackType = Attack.AttackType.Rocket;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			if (attackType == Attack.AttackType.Rocket && GameData.Instance.AllyRocketsAvailable <= 0)
			{
				attackType = Attack.AttackType.AllyGun;
			}

			AttackWith(attackType);
		}
		//if(Input.GetMouseButtonDown(1))
		//{
		//	SteeringVelocity = Vector3.zero;
		//	CurrentVelocity = Vector3.zero;

		//	seekBehaviour.enabled = true;
		//}
		#endregion
	}

	protected override void UpdateDirection()
	{
		if (seekBehaviour.enabled)
		{
			base.UpdateDirection();
		}
		else
		{
			var mouseInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mouseInWorld.z = 0.0f;
			transform.up = Vector3.Normalize(mouseInWorld - transform.position);
		}

		if (GetCurrentDistanceToTarget(targetPosition) <= GetWeaponEngagementRange(attackType))
		{
			AttackWith(attackType);
		}

		AttackWith(attackType);
	}

	private float GetWeaponEngagementRange(Attack.AttackType atk)
	{
		switch (atk)
		{
			case Attack.AttackType.Rocket:
				return Attack.RocketData.range;

			case Attack.AttackType.AllyGun:
				return Attack.AllyGunData.range;

			case Attack.AttackType.Melee:
				return Attack.MeleeData.range;

			default:
				return SteeringAgent.CollisionRadius;
		}
	}

	private float GetCurrentDistanceToTarget(Vector3 targetPos)
	{
		Vector3 dir;
		dir = targetPos - transform.position;

		return dir.magnitude;
	}
}
