using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyScripts.Fundamentals
{
    [DisableAutoCreation]
    // ENABLE for exercise 5 and back
    // DISABLE for exercise 6
    public class MovementSystemBurst : JobComponentSystem
    {
        // =============================================================================================================
        [BurstCompile]
        public struct MovementSystemJob : IJobForEach<Translation, Rotation, MovementVelocity>
        {
            public float deltaTime;

            public void Execute(ref Translation translation, [ReadOnly] ref Rotation rotation,
                [ReadOnly] ref MovementVelocity movementVelocity)
            {
                float3 direction = math.forward(rotation.Value);
                translation.Value += direction * movementVelocity.movingVelocity * deltaTime;
            }
        }
        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples
            Enabled = SceneManager.GetActiveScene().name.Contains("6");
        }
        // =============================================================================================================
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new MovementSystemJob();
            job.deltaTime = Time.deltaTime;

            var jobHandle = job.Schedule(this, inputDeps);
            return jobHandle;
        }
        // =============================================================================================================
    }
}