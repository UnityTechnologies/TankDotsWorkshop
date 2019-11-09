using System;
using Unity.Entities;

namespace Workshop.TankGame
{
    /// <summary>
    /// Our component data for everything that needs to move forward, we are calling it as tag...
    /// ...because we will use it to identify to work with our systems.
    /// And our tag does not need any value on it.
    /// </summary>
    [Serializable]
    public struct MoveForwardTag : IComponentData
    {
    }
}