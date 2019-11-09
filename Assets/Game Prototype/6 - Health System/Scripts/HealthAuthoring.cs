using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class HealthAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Tooltip("Health amount before dying")]
        public float healthAmount = 100;

        // =============================================================================================================
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            Health health = new Health() { lifePoints = healthAmount};
            dstManager.AddComponentData(entity, health);
        }
        // =============================================================================================================
    }
}