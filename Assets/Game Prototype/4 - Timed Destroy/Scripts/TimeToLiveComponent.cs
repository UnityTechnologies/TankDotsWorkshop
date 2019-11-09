using System;
using Unity.Entities;

namespace Workshop.TankGame
{
	/// <summary>
	/// This Component will let us tell how live an Entity can live in our scene, so we can destroy later.
	/// </summary>
	[Serializable]
	public struct TimeToLive : IComponentData
	{
		public float lifeTimer;
	}
}