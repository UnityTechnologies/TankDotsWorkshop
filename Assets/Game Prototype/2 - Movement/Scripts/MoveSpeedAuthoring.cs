using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
    /// <summary>
    /// Our authoring for our MoveSpeed component.
    /// Remember that we are calling as "Authoring" everything that needs to be exposed in Unity Editor.
    /// Here we will convert our normal GameObject (GO) to an Entity.
    /// </summary>
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class MoveSpeedAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Tooltip("Actual speed that we will expose on Unity Editor. It will be called once, you can't make changes" +
                 "during runtime in the Editor.")]
        public float speed = 50f;

        // =============================================================================================================
        /// <summary>
        /// This function will be called when the ECS system decides to convert this GameObject to an equivalent ECS...
        /// ...Entity, allowing us to add the component this MonoBehaviour represents.
        /// </summary>
        /// <param name="entity">Entity to be created for us.</param>
        /// <param name="dstManager">Our entity manager to work with ECS.</param>
        /// <param name="conversionSystem">Not using this for now, used for more advanced stuff.</param>
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //We simply copy our Editor speed to our Component speed.
            MoveSpeed moveSpeed = new MoveSpeed { speedValue = speed };
            dstManager.AddComponentData(entity, moveSpeed);
        }
        // =============================================================================================================
    }
}