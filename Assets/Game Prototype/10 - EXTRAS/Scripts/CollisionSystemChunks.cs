using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

namespace Workshop.TankGame
{
	[DisableAutoCreation]
	[UpdateAfter(typeof(MoveForwardSystem))]
	[UpdateBefore(typeof(TimedDestroySystem))]
	public class CollisionSystemChunks : JobComponentSystem
	{
		EntityQuery enemyGroup;
		EntityQuery bulletGroup;
		EntityQuery playerGroup;

		// =============================================================================================================
		protected override void OnCreate()
		{
			//Dirty Hack for simplifying this samples 
			Enabled = SceneManager.GetActiveScene().name.Contains("10");
			
			playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlayerTag>());
			enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EnemyTag>());
			bulletGroup = GetEntityQuery(typeof(TimeToLive), ComponentType.ReadOnly<Translation>());
		}
		// =============================================================================================================
		//3. The job must be an IJobChunk
		[BurstCompile]
		private struct CollisionJob : IJobChunk
		{
			public float radius;

			public ArchetypeChunkComponentType<Health> healthType;
			[ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;

			[DeallocateOnJobCompletion]
			[ReadOnly] public NativeArray<Translation> transToTestAgainst;
			
			//4. The execute function recives just an entire chunk
			public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
			{
				//5. We request a NativeArray of specific components to the chunk using the Chunk Component Type 
				var chunkHealths = chunk.GetNativeArray(healthType);
				var chunkTranslations = chunk.GetNativeArray(translationType);

				//6. We get the amount of entities using the Count of the chunk
				for (int i = 0; i < chunk.Count; i++)
				{
					float damage = 0f;
					
					//7. We access each component using the index of the entity in the chunk
					Health health = chunkHealths[i];
					Translation pos = chunkTranslations[i];

					for (int j = 0; j < transToTestAgainst.Length; j++)
					{
						Translation pos2 = transToTestAgainst[j];

						if (CheckCollision(pos.Value, pos2.Value, radius))
						{
							damage += 1;
						}
					}

					if (damage > 0)
					{
						health.lifePoints -= damage;
						
						//8. If we change the component we must save it again to the array
						//because this time we dont have a reference to it
						chunkHealths[i] = health;
					}
				}
			}
		}
		// =============================================================================================================
		protected override JobHandle OnUpdate(JobHandle inputDependencies)
		{
			//1. We first need to get the archetype chunk type we will be using
			var healthType = GetArchetypeChunkComponentType<Health>(false);
			var translationType = GetArchetypeChunkComponentType<Translation>(true);

			float enemyRadius = GameSettings.Instance.EnemyCollisionRadius;
			float playerRadius = GameSettings.Instance.PlayerCollisionRadius;

			//2. We pass it to the Job
			var jobEvB = new CollisionJob()
			{
				radius = enemyRadius * enemyRadius,
				healthType = healthType,
				translationType = translationType,
				transToTestAgainst = bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
			};

			JobHandle jobHandle = jobEvB.Schedule(enemyGroup, inputDependencies);

			if (GameSettings.Instance.IsPlayerDead)
				return jobHandle;

			var jobPvE = new CollisionJob()
			{
				radius = playerRadius * playerRadius,
				healthType = healthType,
				translationType = translationType,
				transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
			};

			return jobPvE.Schedule(playerGroup, jobHandle);
		}
		// =============================================================================================================
		static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
		{
			float3 delta = posA - posB;
			float distanceSquare = delta.x * delta.x + delta.z * delta.z;

			return distanceSquare <= radiusSqr;
		}
		// =============================================================================================================
	}
}