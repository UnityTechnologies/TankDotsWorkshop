using Unity.Entities;
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
		/// <summary>
		/// Dirty hack to make it work only at the exercise we want.
		/// </summary>
		private bool onExercise;
		
		// =============================================================================================================
		protected override void OnCreate()
		{
			onExercise = SceneManager.GetActiveScene().name.Contains("10");
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
						
						//ENABLE THIS ONLY AT EXERCISE 10
						if (onExercise)
							LifeCubeSpawner.Instance.SpawnLifeCubeECS(pos.Value);
					}
				}
			});

			// Here on, just execute if we are in Exercise 10.
			if (!onExercise)
				return;
			
			// Same as above, but for HealthCollect (to destroy the life cubes near the player)
			Entities.ForEach((Entity entity, ref HealthCollect healthCollector, ref Translation pos) =>
			{
				if (healthCollector.collected)
				{
					PostUpdateCommands.DestroyEntity(entity);
					Debug.Log("Destroyed LifeCube Entity.");
				}
			});
		}
		// =============================================================================================================
	}
}