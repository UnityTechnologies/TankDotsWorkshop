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
    public class CollisionHealthSystem : JobComponentSystem
    {
        private EntityQuery lifeCubeGroup;
        private EntityQuery playerGroup;

        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples 
            Enabled = SceneManager.GetActiveScene().name.Contains("10");

            playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<PlayerTag>());
            lifeCubeGroup = GetEntityQuery(typeof(HealthCollect), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<LifeCubeTag>());
        }
        // =============================================================================================================
        /// <summary>
        /// A job to that checks all entities with the given components.
        /// </summary>
        [BurstCompile]
        private struct CollisionHealthCollectJob : IJobForEach<Health, Translation>
        {
            public float radius;
            public float amountToRestore;
            public float maxHealthAmount;
            [DeallocateOnJobCompletion, ReadOnly] public NativeArray<Translation> transToTestAgainst;

            public void Execute(ref Health health, [ReadOnly] ref Translation pos)
            {
                float healthAmount = 0f;
                for (var j = 0; j < transToTestAgainst.Length; j++)
                {
                    Translation pos2 = transToTestAgainst[j];
                    if (CheckCollision(pos.Value, pos2.Value, radius))
                        healthAmount = amountToRestore;
                }

                if (healthAmount > 0)
                {
                    health.lifePoints += healthAmount;
                    if (health.lifePoints > maxHealthAmount)
                        health.lifePoints = maxHealthAmount;
                }
            }
        }
        // =============================================================================================================
        /// <summary>
        /// A job to that checks all entities with the given components.
        /// </summary>
        [BurstCompile]
        private struct CollisionLifeCubeCollectedJob : IJobForEach<HealthCollect, Translation>
        {
            public float radius;
            [DeallocateOnJobCompletion, ReadOnly] public NativeArray<Translation> transToTestAgainst;

            public void Execute(ref HealthCollect healthCollected, [ReadOnly] ref Translation pos)
            {
                for (var j = 0; j < transToTestAgainst.Length; j++)
                {
                    Translation pos2 = transToTestAgainst[j];
                    if (CheckCollision(pos.Value, pos2.Value, radius))
                        healthCollected.collected = true;
                }
            }
        }
        // =============================================================================================================
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float playerRadius = GameSettings.Instance.PlayerCollisionRadius;
            
            //A job to collect health to check against all players
            var jobCollectHealth = new CollisionHealthCollectJob()
            {
                radius = playerRadius * playerRadius,
                amountToRestore = GameSettings.Instance.LifeCubeRestoreAmount,
                maxHealthAmount = GameSettings.Instance.PlayerMaxHealth,
                transToTestAgainst = lifeCubeGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
            };
            
            //Add it to our job schedule
            JobHandle jobCollectHealthHandle = jobCollectHealth.Schedule(playerGroup, inputDependencies);
            
            //If player is already dead, return our handle, do nothing.
            if (GameSettings.Instance.IsPlayerDead)
                return jobCollectHealthHandle;
            
            //A job to mark if the LifeCube was already collected
            var jobMarkLifeCubeCollected = new CollisionLifeCubeCollectedJob()
            {
                radius = playerRadius * playerRadius,
                transToTestAgainst = playerGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
            };

            //Add it to our job schedule
            return jobMarkLifeCubeCollected.Schedule(lifeCubeGroup, jobCollectHealthHandle);
        }
        // =============================================================================================================
        private static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
        {
            float3 delta = posA - posB;
            float distanceSquare = delta.x * delta.x + delta.z * delta.z;
            return distanceSquare <= radiusSqr;
        }
        // =============================================================================================================
    }
}