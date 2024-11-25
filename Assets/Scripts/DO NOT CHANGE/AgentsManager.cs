using System;
using System.Collections.Generic;
using UnityEngine;

public class AgentsManager
{
	public float GetAgentHealth(SteeringAgent agent)
	{
		if (agentToHealth.ContainsKey(agent))
		{
			return agentToHealth[agent];
		}
		return -1.0f;
	}

	/// <summary>
	/// Recreates ally units at the map index locations as keys in the dictionary to the class types you want to create.
	/// NOTE: use Map.MapIndex(int x, int y) to convert agent x, y coordinates to a map index
	/// NOTE: this will only work before the first update cycle so its important to call this as soon as possible
	/// </summary>
	/// <param name="agentIndexToAgentType">Map index to the agent type to create. Use typeof(YourClassName) to get the Type</param>
	public void CreateAllyAgents(Dictionary<int, Type> agentIndexToAgentType)
	{
		if (hasFristUpdateExcuted)
		{
			Debug.LogError("CreateAllyUints needs to be called before the first update cycle is executed. Ensure this called in a Start function and that the GameObject is in the Heirarchy from the beginning,");
			return;
		}

		foreach (var ally in allyAgents)
		{
			GameObject.Destroy(ally.gameObject);
		}
		allyAgents.Clear();

		var map = GameData.Instance.Map;
		var allyLocations = map.GetInitialAllyLocations();
		foreach (var allyLocation in allyLocations)
		{
			allyAgents.Add(CreateAgent(map, map.MapIndexToX(allyLocation), map.MapIndexToY(allyLocation), false, agentIndexToAgentType[allyLocation]));
		}

		CopyUnitsToLists();
	}

	#region Private interface

	private Dictionary<SteeringAgent, float> agentToHealth = new Dictionary<SteeringAgent, float>();

	private List<SteeringAgent> enemyAgents = new List<SteeringAgent>();
	private List<SteeringAgent> allyAgents = new List<SteeringAgent>();

	private Sprite agentSprite;

	private GameObject enemiesGO;
	private GameObject alliesGO;

	private bool hasFristUpdateExcuted = false;

	public void Initialise()
	{
		enemiesGO = new GameObject("Enemies");
		alliesGO = new GameObject("Allies");

		agentSprite = Resources.Load<Sprite>("Unit");

		var gameData = GameData.Instance;
		var map = gameData.Map;

		var enemyType = typeof(EnemyAgent);
		var enemyUnitLocations = map.GetInitialEnemyLocations();
		foreach (var enemyLocation in enemyUnitLocations)
		{
			enemyAgents.Add(CreateAgent(map, map.MapIndexToX(enemyLocation), map.MapIndexToY(enemyLocation), true, enemyType));
		}

		if (allyAgents.Count <= 0)
		{
			var allyType = typeof(AllyAgent);
			var allyLocations = map.GetInitialAllyLocations();
			foreach (var allyLocation in allyLocations)
			{
				allyAgents.Add(CreateAgent(map, map.MapIndexToX(allyLocation), map.MapIndexToY(allyLocation), false, allyType));
			}
		}

		CopyUnitsToLists();
	}

	public void Tick()
	{
		hasFristUpdateExcuted = true;
		CopyUnitsToLists();
	}

	private void CopyUnitsToLists()
	{
		var gameData = GameData.Instance;
		gameData.allies.Clear();
		foreach (var unit in allyAgents)
		{
			gameData.allies.Add(unit);
		}
		gameData.enemies.Clear();
		foreach (var unit in enemyAgents)
		{
			gameData.enemies.Add(unit);
		}
	}


	private SteeringAgent CreateAgent(Map map, int mapX, int mapY, bool isEnemy, Type type)
	{
		if (!typeof(SteeringAgent).IsAssignableFrom(type))
		{
			throw new TypeAccessException("Type " + type.Name + " does not derive from " + typeof(SteeringAgent).Name);
		}

		var agentGO = new GameObject(isEnemy ? "Enemy " + enemyAgents.Count : "Ally " + allyAgents.Count);
		agentGO.transform.parent = isEnemy ? enemiesGO.transform : alliesGO.transform;
		agentGO.transform.position = new Vector3(mapX, mapY, 0.0f);

		var spriteRenderer = agentGO.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = agentSprite;
		spriteRenderer.color = isEnemy ? Color.yellow : Color.magenta;

		var collider = agentGO.AddComponent<CircleCollider2D>();
		collider.radius = SteeringAgent.CollisionRadius;

		// Ensure this is last as the users entry point into these classes will be called when this happens
		// so everything needs setup before this
		var agent = agentGO.AddComponent(type) as SteeringAgent;
		agentToHealth.Add(agent, 1.0f);
		return agent;
	}

	/// <summary>
	/// NEVER CALL THIS!
	/// Anyone calling this function manually will be deducted marks from their coursework!
	/// Applies damage to the agent
	/// </summary>
	/// <param name="agent">Agent to apply damage to</param>
	/// <param name="damage">Amount of damage to apply</param>
	public void ApplyDamageToAgent(SteeringAgent agent, float damage)
	{
		if (agentToHealth.ContainsKey(agent))
		{
			agentToHealth[agent] -= damage;

			if (agentToHealth[agent] <= 0.0f)
			{
				agentToHealth.Remove(agent);
			}
		}
	}
	#endregion
}
