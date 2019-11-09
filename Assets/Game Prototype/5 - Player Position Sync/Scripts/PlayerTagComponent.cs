using System;
using Unity.Entities;

namespace Workshop.TankGame
{
    /// <summary>
    /// This will be used to identify our player in game.
    /// </summary>
    [Serializable]
    public struct PlayerTag : IComponentData
    {
    }
}