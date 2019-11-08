using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// For more info about systems checkout Fundamentals - Exercise 3
    /// </summary>
    public class MovementSystemForEach : ComponentSystem
    {
        // =============================================================================================================
        /// <summary>
        /// This function executes once when the system is created to initialize the system.
        /// </summary>
        protected override void OnCreate()
        {
            //Dirty Hack for simplifying this samples.
            //We want this to work only if we are on Exercise 3 or 4, because the Component System will be...
            //... explained only by there and used there only.
            Enabled = SceneManager.GetActiveScene().name.Contains("3") ||
                      SceneManager.GetActiveScene().name.Contains("4");
        }
        // =============================================================================================================
        /// <summary>
        /// This will be executed in all frames.
        /// </summary>
        protected override void OnUpdate()
        {
            //Just another way of executing a system query using lambdas.
            Entities.ForEach((Entity entity, ref Translation translation, ref Rotation rotation,
                ref MovementVelocity movementVelocity) =>
            {
                //Create the forward direction based on the current rotation.
                var direction = math.forward(rotation.Value); //float3 type

                //Apply a translation along the forward direction.
                translation.Value += direction * movementVelocity.movingVelocity * Time.deltaTime;
            });
        }
        // =============================================================================================================
    }
}