using System;
using Unity.Entities;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// Components are just structs WITH THE DATA (Runtime Component).
    /// Later, we create a MonoBehaviour that just creates it and adds it an Entity (Authoring Component).
    /// That means we use the MonoBehaviour just for integration with the editor.
    /// 
    /// This struct represents an actual ECS component to attach to an Entity.
    /// Notice that this is an struct instead of a class, meaning that this will be passed by value later.
    /// </summary>
    [Serializable]
    public struct RotationVelocity : IComponentData
    {
        //The rotation velocity of our cube.
        public float rotatingVelocity; //picked a big name, so you can see the difference.
    }
}