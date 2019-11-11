using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

namespace Workshop.TankGame
{
	/// <summary>
	/// Custom solution for collision in our game prototype, do not use this in real life, use Unity Physics that...
	/// ...is almost here.
	/// </summary>
	public class CollisionSystem : JobComponentSystem
	{
		private EntityQuery enemyGroup;

		// =============================================================================================================
		protected override void OnCreate()
		{
			//Dirty Hack for simplifying this samples - we want this to work only in this scene.
			Enabled = SceneManager.GetActiveScene().name.Contains("7");
			//Let's create a query filter to access a specific group of entities.
			//We can execute a Query manually.
			//We can get all entities that comply it.
			//We can also get specifics components that comply to it.
			enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(),
				ComponentType.ReadOnly<EnemyTag>());
		}
		// =============================================================================================================
		/// <summary>
		/// Our job will search for all entities with Health and Translation components.
		/// Then check if a collision happened and execute something on the Health component.
		/// In this example, we will make the player damage our enemies by "touching" them.
		/// </summary>
		[BurstCompile]
		private struct CollisionJob : IJobForEach<Health, Translation>
		{
			public float radius;
			public float3 playerPosition;

			public void Execute(ref Health health, [ReadOnly] ref Translation pos)
			{
				if (CheckCollision(pos.Value, playerPosition, radius))
					health.lifePoints -= 1;
			}
		}
		// =============================================================================================================
		/// <summary>
		/// Our Job Handle can get info from our MonoBehaviour objects.
		/// After we get the info, we send to our selected group in our query.
		/// </summary>
		/// <param name="inputDependencies"></param>
		/// <returns></returns>
		protected override JobHandle OnUpdate(JobHandle inputDependencies)
		{
			var jobEvP = new CollisionJob();
			jobEvP.radius = GameSettings.Instance.EnemyCollisionRadius;
			jobEvP.playerPosition = GameSettings.Instance.PlayerPosition;
			return jobEvP.Schedule(enemyGroup, inputDependencies);
		}
		// =============================================================================================================
		/// <summary>
		/// This is our custom collision calculation. We will do it for all entities that has the job requirements.
		/// It does not matter if it is really near of not, it will calculate.
		/// That's why is not good to use it in a final game.
		/// </summary>
		/// <param name="posA"></param>
		/// <param name="posB"></param>
		/// <param name="radiusSqr"></param>
		/// <returns></returns>
		private static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
		{
			float3 delta = posA - posB;
			float distanceSquare = delta.x * delta.x + delta.z * delta.z;
			return distanceSquare <= radiusSqr;
		}
		// =============================================================================================================
	}
}