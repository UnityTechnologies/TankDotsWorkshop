using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;

namespace Workshop.TankGame
{
	/// <summary>
	/// This system does not need to be a Job Component System, because only our player should access in our game.
	/// </summary>
	public class PlayerTransformUpdateSystem : ComponentSystem
	{
		// =============================================================================================================
		protected override void OnUpdate()
		{
			if (GameSettings.Instance == null)
				return;
			
			if (GameSettings.Instance.IsPlayerDead)
				return;
			
			//Set our ECS position the same as our GO position
			Entities.WithAll<PlayerTag>().ForEach((ref Translation pos) =>
			{
				pos = new Translation { Value = GameSettings.Instance.PlayerPosition};
			});
		}
		// =============================================================================================================
	}
}