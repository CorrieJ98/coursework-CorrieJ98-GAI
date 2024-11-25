using UnityEngine;

public class AllyAgent : SteeringAgent
{
	// placeholder for later
	// private enum AllyAgentFSM {IDLE, MOVING, ENGAGE_ENEMY, RETREAT, DODGE}

	private Pathfinder allyPathfinder;

	private AllyAgentSeek seekBehaviour;
	private AllyAgentFlee fleeBehaviour;
	private AllyAgentFleeRocket fleeRocketBehaviour;
	private AllyAgentDodge dodgeBehaviour;

    public SteeringAgent nearestAlly;
    public Vector3 rocketPosition;
	public Vector3 startPosition;

    private Attack.AttackType attackType = Attack.AttackType.AllyGun;

	protected override void InitialiseFromAwake()
	{
		allyPathfinder = gameObject.AddComponent<Pathfinder>();
        seekBehaviour = gameObject.AddComponent<AllyAgentSeek>();
		fleeBehaviour= gameObject.AddComponent<AllyAgentFlee>();
		fleeRocketBehaviour = gameObject.AddComponent<AllyAgentFleeRocket>();
		dodgeBehaviour= gameObject.AddComponent<AllyAgentDodge>();
    }

	protected override void CooperativeArbitration()
	{
		base.CooperativeArbitration();

		// keybindings
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
			if(attackType == Attack.AttackType.Rocket && GameData.Instance.AllyRocketsAvailable <= 0)
			{
				attackType = Attack.AttackType.AllyGun;
			}

			AttackWith(attackType);
		}
		if(Input.GetMouseButtonDown(1))
		{
			SteeringVelocity = Vector3.zero;
			CurrentVelocity = Vector3.zero;

			seekBehaviour.enabled = true;
		}
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
	}
}
