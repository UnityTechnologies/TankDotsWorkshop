using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyScripts.Fundamentals
{
    /// <summary>
    /// This class represents a system that will execute certain logic every frame.
    /// </summary>
    // [DisableAutoCreation] // This attribute will disable the system
    // COMMENT for exercise 4 and back
    // UNCOMMENT for exercise 5 and 6
    //
    // But what is DisableAutoCreation:
    // - Prevents systems from automatically being instantiated for a default world:
    // - https://docs.unity3d.com/Packages/com.unity.entities@0.1/api/Unity.Entities.DisableAutoCreationAttribute.html
    // - Systems marked as [DisableAutoCreation] can be created manually, updated manually (by calling
    // MySystem.Update()), and even manually added to a ComponentSystemGroup's update list (in which case they will be
    // updated as part of that group's OnUpdate() call). The attribute just means that none of that happens
    // automatically during default World initialization.
    public class RotationSystemForEach : ComponentSystem
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
            // This function will find all entities with Rotation and RotationVelocity components
            // and will execute the function "RotateEntity" per each one.
            Entities.ForEach<Rotation, RotationVelocity>(RotateEntity);
        }
        // =============================================================================================================
        /// <summary>
        /// This function has the logic to apply to all entities with Rotation and RotationVelocity...
        /// ...components every frame
        /// </summary>
        /// <param name="entity">Entity that will come from our for each</param>
        /// <param name="rotation"></param>
        /// <param name="rotationVelocity"></param>
        private void RotateEntity(Entity entity, ref Rotation rotation, ref RotationVelocity rotationVelocity)
        {
            //We get the velocity specified in the component, apply delta time and convert degrees to radians.
            float rotationAmount = rotationVelocity.rotatingVelocity * Time.deltaTime * Mathf.Deg2Rad;

            //Create a quaternion that represents the desired amount of rotation.
            //Notice that this quaternion is not the same as the default Quaternion of MonoBehaviours.
            quaternion rotationQuat = quaternion.Euler(0, rotationAmount, 0);

            //Apply the desired rotation multiplying current rotation by the quaternion 
            quaternion newRotation = math.mul(rotation.Value, rotationQuat);

            //Override the current rotation with the new rotated rotation;
            rotation.Value = newRotation;
        }
        // =============================================================================================================
    }
}