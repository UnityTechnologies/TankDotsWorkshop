using System;
using Unity.Entities;

namespace MyScripts.Fundamentals
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