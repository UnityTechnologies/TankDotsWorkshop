using System;
using Unity.Entities;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// Check RotationVelocity for explanation about Components.
    /// </summary>
    [Serializable]
    public struct MovementVelocity : IComponentData
    {
        public float movingVelocity;
    }
}