using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class LifeCubeAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        // =============================================================================================================
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent(entity, typeof(LifeCubeTag));
        }
        // =============================================================================================================
    }
}