using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Workshop.Fundamentals
{
    [DisableAutoCreation]
    // COMMENT for exercise 6 ONLY
    // UNCOMMENT for all other exercises
    public class RotationSystemBurst : JobComponentSystem
    {
        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples
            Enabled = SceneManager.GetActiveScene().name.Contains("6");
        }
        // =============================================================================================================
        [BurstCompile]
        public struct RotationSystemJob : IJobForEach<Rotation, RotationVelocity>
        {
            public float deltaTime;
        
            public void Execute(ref Rotation rotation, [ReadOnly] ref RotationVelocity rotationVelocity)
            {
                float rotationAmount = rotationVelocity.rotatingVelocity * deltaTime * Mathf.Deg2Rad;
                quaternion rotationQuat = quaternion.Euler(0, rotationAmount, 0);
                quaternion newRotation = math.mul(rotationQuat, rotation.Value);
                rotation.Value = newRotation;
            }
        }
        // =============================================================================================================
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new RotationSystemJob();
            job.deltaTime = Time.deltaTime;

            var jobHandle = job.Schedule(this, inputDeps);
            return jobHandle;
        }
        // =============================================================================================================
    }
}