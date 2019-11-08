using Unity.Entities;
using UnityEngine;

namespace Workshop.TankGame
{
	/// <summary>
	/// Our authoring for our MoveForward component.
	/// Remember that we are calling as "Authoring" everything that needs to be exposed in Unity Editor.
	/// And this one will be used as a tag only.
	/// Here we will convert our normal GameObject (GO) to an Entity.
	/// </summary>
	[DisallowMultipleComponent]
	[RequiresEntityConversion]
	public class MoveForwardAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		// =============================================================================================================
		public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
		{
			manager.AddComponent(entity, typeof(MoveForwardTag));
		}
		// =============================================================================================================
	}
}