using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace Workshop.TankGame
{
	/// <summary>
	/// We will update some of our player ECS variables to our normal MB, so we can see changes like in UI.
	/// There are a number of ways for doing this, but just to show a different example, we decided to do this.
	/// </summary>
	public class UpdatePlayerMbToECS : ComponentSystem
	{
		// =============================================================================================================
		protected override void OnCreate()
		{
			Enabled = SceneManager.GetActiveScene().name.Contains("10");
		}
		// =============================================================================================================
		protected override void OnUpdate()
		{
			// This function will find all entities with Health and Translation components
			// and will execute the code below.
			Entities.ForEach((Entity entity, ref Health health, ref Translation pos) =>
			{
				if (EntityManager.HasComponent(entity, typeof(PlayerTag)))
				{
					GameSettings.Instance.UpdatePlayerCurrentHealth(health.lifePoints);
				}
			});
		}
		// =============================================================================================================
	}
}