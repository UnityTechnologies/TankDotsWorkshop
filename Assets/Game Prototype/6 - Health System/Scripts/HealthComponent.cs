using System;
using Unity.Entities;

namespace Workshop.TankGame
{
	[Serializable]
	public struct Health : IComponentData
	{
		public float lifePoints;
	}
}