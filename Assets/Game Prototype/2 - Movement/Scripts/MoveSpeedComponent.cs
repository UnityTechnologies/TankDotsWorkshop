using System;
using Unity.Entities;

namespace Workshop.TankGame
{
	/// <summary>
	/// Our component data for everything that needs a speed component.
	/// This one we added a value, although we don't need it always (i.e. if we want to create as a tag).
	/// </summary>
	[Serializable]
	public struct MoveSpeed : IComponentData
	{
		/// <summary>
		/// The move speed value, custom created.
		/// </summary>
		public float speedValue;
	}
}