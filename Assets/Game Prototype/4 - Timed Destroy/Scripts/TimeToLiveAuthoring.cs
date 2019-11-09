using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class TimeToLiveAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Tooltip("How long should I live before destroying myself?")]
        public float remainingTime;

        // =============================================================================================================
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TimeToLive { lifeTimer = remainingTime});
        }
        // =============================================================================================================
    }
}