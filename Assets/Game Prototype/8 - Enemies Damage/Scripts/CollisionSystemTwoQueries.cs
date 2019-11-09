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
    public class CollisionSystemTwoQueries : JobComponentSystem
    {
        private EntityQuery enemyGroup;
        private EntityQuery playerGroup;

        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples 
            Enabled = SceneManager.GetActiveScene().name.Contains("8");

            //More queries to get the groups we want.
            playerGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<PlayerTag>());
            enemyGroup = GetEntityQuery(typeof(Health), ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<EnemyTag>());
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
                    {
                        damage += 1;
                    }
                }
                if (damage > 0)
                    health.lifePoints -= damage;
            }
        }
        // =============================================================================================================
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var jobEvB = new CollisionJob();
            jobEvB.radius = GameSettings.Instance.EnemyCollisionRadius;
            jobEvB.transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob);
            return jobEvB.Schedule(playerGroup, inputDependencies);
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