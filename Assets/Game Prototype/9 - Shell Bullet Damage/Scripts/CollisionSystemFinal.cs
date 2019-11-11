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
    public class CollisionSystemFinal : JobComponentSystem
    {
        private EntityQuery enemyGroup;
        private EntityQuery bulletGroup;
        private EntityQuery playerGroup;

        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples 
            Enabled = SceneManager.GetActiveScene().name.Contains("9") || SceneManager.GetActiveScene().name.Contains("10");

            playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<PlayerTag>());
            enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<EnemyTag>());
            bulletGroup = GetEntityQuery(typeof(TimeToLive), ComponentType.ReadOnly<Translation>());
        }
        // =============================================================================================================
        [BurstCompile]
        private struct CollisionJob : IJobForEach<Health, Translation>
        {
            public float radius;
            [DeallocateOnJobCompletion, ReadOnly] 
            public NativeArray<Translation> transToTestAgainst;

            public void Execute(ref Health health, [ReadOnly] ref Translation pos)
            {
                float damage = 0f;
                for (var j = 0; j < transToTestAgainst.Length; j++)
                {
                    Translation pos2 = transToTestAgainst[j];
                    if (CheckCollision(pos.Value, pos2.Value, radius))
                        damage += 1;
                }
                if (damage > 0)
                    health.lifePoints -= damage;
            }
        }
        // =============================================================================================================
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float enemyRadius = GameSettings.Instance.EnemyCollisionRadius;
            float playerRadius = GameSettings.Instance.PlayerCollisionRadius;

            //A job for our enemies against all bullets
            var jobEvB = new CollisionJob()
            {
                radius = enemyRadius * enemyRadius,
                transToTestAgainst = bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
            };

            //Add it to our job schedule
            JobHandle jobEvBHandle = jobEvB.Schedule(enemyGroup, inputDependencies);

            //If player is already dead, return our handle, do nothing.
            if (GameSettings.Instance.IsPlayerDead)
                return jobEvBHandle;

            //A job for our player against all enemies
            var jobPvE = new CollisionJob()
            {
                radius = playerRadius * playerRadius,
                transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
            };

            //Add it to our job schedule
            return jobPvE.Schedule(playerGroup, jobEvBHandle);
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