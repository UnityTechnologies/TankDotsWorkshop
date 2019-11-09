using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.SceneManagement;

namespace Workshop.TankGame
{
    public class CollisionSystemFinal : JobComponentSystem
    {
        private EntityQuery enemyGroup;
        private EntityQuery bulletGroup;
        private EntityQuery playerGroup;

        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples 
            Enabled = SceneManager.GetActiveScene().name.Contains("9");

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
                }
            }
        }
        // =============================================================================================================
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            float enemyRadius = GameSettings.Instance.EnemyCollisionRadius;
            float playerRadius = GameSettings.Instance.PlayerCollisionRadius;

            var jobEvB = new CollisionJob()
            {
                radius = enemyRadius * enemyRadius,
                transToTestAgainst = bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
            };

            JobHandle jobEvBHandle = jobEvB.Schedule(enemyGroup, inputDependencies);

            if (GameSettings.Instance.IsPlayerDead)
                return jobEvBHandle;

            var jobPvE = new CollisionJob()
            {
                radius = playerRadius * playerRadius,
                transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
            };

            return jobPvE.Schedule(playerGroup, jobEvBHandle);

            //var jobPvEHandle = jobPvE.Schedule(playerGroup, inputDependencies);
            //return JobHandle.CombineDependencies(jobEvBHandle, jobPvEHandle);
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