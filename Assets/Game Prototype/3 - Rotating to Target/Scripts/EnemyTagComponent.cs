using System;
using Unity.Entities;

namespace Workshop.TankGame
{
    /// <summary>
    /// Our component data for everything that needs be considered an Enemy, we are calling it as tag...
    /// ...because we will use it as a tag to work with our systems.
    /// And our tag does not need any value on it (but it could).
    /// </summary>
    [Serializable]
    public struct EnemyTag : IComponentData
    {
    }
}