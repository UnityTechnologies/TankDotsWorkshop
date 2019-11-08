using Unity.Entities;
using UnityEngine;

namespace MyScripts.Fundamentals
{
    /// <summary>
    /// This MonoBehaviour is JUST USED for editor integration purposes until we have a proper ECS editor tools.
    /// This will be destroyed after the creation of the actual ECS components happens.
    /// Notice the IConvertGameObjectToEntity interface that makes us to have the Convert Function.
    /// </summary>
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class RotationVelocityAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        //IConvertGameObjectToEntity will, as it says, convert our GO to an entity and expose what we need here.
        //We expose this in the editor just to configure the actual ECS component we will create later.
        public float rotationVelocity;

        //This function will be called when the ECS system decides to convert this GameObject to an equivalent ECS...
        //...Entity, allowing us to add the component this MonoBehaviour represents.
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //First create the actual component and setup its data based on exposed properties.
            var rotationComponent = new RotationVelocity();
            //Notice that this is not a reference, but a value that will be copied to the Entity Manager.
            rotationComponent.rotatingVelocity = rotationVelocity;

            //Tell the entity manager that this Component belongs to the entity we are converting.
            dstManager.AddComponentData(entity, rotationComponent);
        }
    }
}