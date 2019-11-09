using Unity.Entities;
using Unity.Transforms;

namespace Workshop.TankGame
{
	public class RemoveDeadSystem : ComponentSystem
	{
		// =============================================================================================================
		protected override void OnUpdate()
		{
			// This function will find all entities with Health and Translation components
			// and will execute the code below.
			Entities.ForEach((Entity entity, ref Health health, ref Translation pos) =>
			{
				if (health.lifePoints <= 0)
				{
					if (EntityManager.HasComponent(entity, typeof(PlayerTag)))
					{
						GameSettings.Instance.PlayerDied();
					}
					else if (EntityManager.HasComponent(entity, typeof(EnemyTag)))
					{
						PostUpdateCommands.DestroyEntity(entity);
						BulletImpactPool.Instance.PlayBulletImpact(pos.Value);
					}
				}
			});
		}
		// =============================================================================================================
	}
}