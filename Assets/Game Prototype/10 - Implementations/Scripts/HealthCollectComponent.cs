using System;
using Unity.Entities;

namespace Workshop.TankGame
{
    [Serializable]
    public struct HealthCollect : IComponentData
    {
        public float amountToRestore;
        public bool collected;
    }
}