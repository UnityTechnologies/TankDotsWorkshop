using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
	/// <summary>
	/// Our authoring for our MoveForward component.
	/// Remember that we are calling as "Authoring" everything that needs to be exposed in Unity Editor.
	/// Here we will convert our normal GameObject (GO) to an Entity.
	/// </summary>
	[DisallowMultipleComponent]
	[RequiresEntityConversion]
	// [DisallowMultipleComponent, RequiresEntityConversion] //This also works as above
	public class MoveForwardAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		// =============================================================================================================
		/// <summary>
		/// This function will be called when the ECS system decides to convert this GameObject to an equivalent ECS...
		/// ...Entity, allowing us to add the component this MonoBehaviour represents.
		/// </summary>
		/// <param name="entity">Entity to be created for us.</param>
		/// <param name="manager">Our entity manager to work with ECS.</param>
		/// <param name="conversionSystem">Not using this for now, used for more advanced stuff.</param>
		public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
		{
			manager.AddComponent(entity, typeof(MoveForwardTag));
		}
		// =============================================================================================================
	}
}