using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class HealthCollectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Tooltip("Health amount to give to player dying")]
        public float restoreAmount = 10;
        
        [Tooltip("It will be marked when player collects it.")]
        public bool playerCollected;

        // =============================================================================================================
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            HealthCollect healthCollector = new HealthCollect
            {
                amountToRestore = restoreAmount,
                collected = playerCollected
            };
            dstManager.AddComponentData(entity, healthCollector);
        }
        // =============================================================================================================
    }
}