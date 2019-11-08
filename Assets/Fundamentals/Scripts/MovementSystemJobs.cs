using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Workshop.Fundamentals
{
    [DisableAutoCreation]
    public class MovementSystemJobs : JobComponentSystem
    {
        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples
            Enabled = SceneManager.GetActiveScene().name.Contains("5");
        }
        // =============================================================================================================
        public struct MovementSystemJob : IJobForEach<Translation, Rotation, MovementVelocity>
        {
            public float deltaTime;

            public void Execute(ref Translation translation, ref Rotation rotation,
                ref MovementVelocity movementVelocity)
            {
                float3 direction = math.forward(rotation.Value);
                translation.Value += direction * movementVelocity.movingVelocity * deltaTime;
            }
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