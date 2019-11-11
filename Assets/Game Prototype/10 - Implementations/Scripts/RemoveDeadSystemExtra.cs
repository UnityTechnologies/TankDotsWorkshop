using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Workshop.TankGame
{
	/// <summary>
	/// We could have done this using Jobs, but we wanted to let you know how to use "HasComponent".
	/// Also, this one is an update on the RemoveDeadSystem script, but with our final implementations.
	/// </summary>
	public class RemoveDeadSystemExtra : ComponentSystem
	{
		// =============================================================================================================
		protected override void OnCreate()
		{
			//Dirty hack to work with our samples. This one to work on scene 10 only.
			Enabled = SceneManager.GetActiveScene().name.Contains("10");
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
						Debug.Log("Destroyed ENEMY entity.");
						BulletImpactPool.Instance.PlayBulletImpact(pos.Value);
						//Let's spawn our life cube.
						LifeCubeSpawner.Instance.SpawnLifeCubeECS(pos.Value);
						//If we destroyed an enemy, let's add score to the player
						GameSettings.Instance.AddScore();
					}
				}
			});
			// Same as above, but for HealthCollect (to destroy the life cubes near the player).
			Entities.ForEach((Entity entity, ref HealthCollect healthCollector, ref Translation pos) =>
			{
				if (healthCollector.collected)
				{
					GameSettings.Instance.PlayCollectAudio();
					PostUpdateCommands.DestroyEntity(entity);
					Debug.Log("Destroyed LifeCube Entity.");
				}
			});
		}
		// =============================================================================================================
	}
}