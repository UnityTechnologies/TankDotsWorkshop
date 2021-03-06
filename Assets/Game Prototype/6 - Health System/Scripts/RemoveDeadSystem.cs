﻿using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Workshop.TankGame
{
	/// <summary>
	/// We could have done this using Jobs, but we wanted to let you know how to use "HasComponent".
	/// </summary>
	public class RemoveDeadSystem : ComponentSystem
	{
		// =============================================================================================================
		protected override void OnCreate()
		{
			//If it is in our final scene, ignore.
			Enabled = !SceneManager.GetActiveScene().name.Contains("10");
		}
		// =============================================================================================================
		protected override void OnUpdate()
		{
			// This function will find all entities with Health and Translation components
			// and will execute the code below.
			Entities.ForEach((Entity entity, ref Health health, ref Translation pos) =>
			{
				if (health.lifePoints <= 0)
				{
					if (EntityManager.HasComponent(entity, typeof(PlayerTag)))
					{
						GameSettings.Instance.PlayerDied();
					}
					else if (EntityManager.HasComponent(entity, typeof(EnemyTag)))
					{
						PostUpdateCommands.DestroyEntity(entity);
						Debug.Log("Destroyed entity.");
						BulletImpactPool.Instance.PlayBulletImpact(pos.Value);
					}
				}
			});
		}
		// =============================================================================================================
	}
}