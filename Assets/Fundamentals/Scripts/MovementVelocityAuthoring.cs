using Unity.Entities;
using UnityEngine;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// Please refer to Fundamentals - Exercise 2 to find more info about creating authoring components.
    /// Or check RotationVelocityAuthoring to see more.
    /// </summary>
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class MovementVelocityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Tooltip("Our velocity when it moves.")]
        [SerializeField] private float moveVelocity;

        // =============================================================================================================
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //Just a shorter way to create the component and adding it to the entity.
            dstManager.AddComponentData(entity, new MovementVelocity { movingVelocity = moveVelocity});
        }
        // =============================================================================================================
    }
}