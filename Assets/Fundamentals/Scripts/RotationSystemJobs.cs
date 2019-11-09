using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// JobComponentSystem enable us to create jobs with Job System.
    /// Turning this into a JobComponentSystem will make Unity to split our job in different threads for us, faster.
    /// </summary>
    [DisableAutoCreation]
    // COMMENT for exercise 5 ONLY
    // UNCOMMENT for all other exercises
    public class RotationSystemJobs : JobComponentSystem
    {
        // =============================================================================================================
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples
            Enabled = SceneManager.GetActiveScene().name.Contains("5");
        }
        // =============================================================================================================
        /// <summary>
        /// This will be our job to run in our JobComponentSystem. We could create more than one, as many as we want.
        /// First part of it is a query - what data you are operating on and, for execution, what you do to that data?
        /// And our query says "find everything that has a Rotation and a RotationVelocity".
        /// Note 2: you can't access Unity normal components and MonoBehaviours from here (check deltaTime below).
        /// </summary>
        public struct RotationSystemJob : IJobForEach<Rotation, RotationVelocity>
        {
            public float deltaTime;

            /// <summary>
            /// Each entity that has these data, will be executed by our query.
            /// It will happen for each data.
            /// </summary>
            /// <param name="rotation"></param>
            /// <param name="rotationVelocity"></param>
            public void Execute(ref Rotation rotation, ref RotationVelocity rotationVelocity)
            {
                //We can't use Time.deltaTime here, since it only be called from the main thread.
                //If you do, even if you don't get a compile error here, it will crash during runtime.
                float rotationAmount = rotationVelocity.rotatingVelocity * deltaTime * Mathf.Deg2Rad;
                quaternion rotationQuat = quaternion.Euler(0, rotationAmount, 0);
                quaternion newRotation = math.mul(rotationQuat, rotation.Value);
                rotation.Value = newRotation;
            }
        }
        // =============================================================================================================
        /// <summary>
        /// This runs our jobs and reads in our input dependencies and return to us.
        /// </summary>
        /// <param name="inputDeps">Input dependencies</param>
        /// <returns></returns>
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