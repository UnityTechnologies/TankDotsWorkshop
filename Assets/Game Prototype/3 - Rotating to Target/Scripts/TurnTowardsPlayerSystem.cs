using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Workshop.TankGame
{
	/// <summary>
	/// REMEMBER: besides the name here, it is actually "turn to something" system, player is just the name we gave.
	/// We are using the "Player" in the name below just to be easier to understand in our game.
	/// A Job Component System does not know what is a player, unless we explicit tell it so.
	/// </summary>
	public class TurnTowardsPlayerSystem : JobComponentSystem
	{
		// =============================================================================================================
		/// <summary>
		/// Let's get ALL entities that has a Translation AND a Rotation Component, to execute our query.
		/// But ALSO, it should ONLY work if it has an Enemy Tag (also a Component).
		/// </summary>
		[BurstCompile]
		[RequireComponentTag(typeof(EnemyTag))] //REMOVE this to see what happens with the Enemy AND the Shells (bullets)
		struct TurnJob : IJobForEach<Translation, Rotation>
		{
			public float3 playerPosition;

			/// <summary>
			/// Every job needs an Execute method.
			/// </summary>
			/// <param name="pos">This is read only because we will not going to change it's value.</param>
			/// <param name="rot">Our rotation, value will change.</param>
			public void Execute([ReadOnly] ref Translation pos, ref Rotation rot)
			{
				//Normal rotation math, boilerplate code
				float3 heading = playerPosition - pos.Value;
				heading.y = 0f;
				rot.Value = quaternion.LookRotation(heading, math.up());
			}
		}
		// =============================================================================================================
		/// <summary>
		/// The query above (our Job) will be called here.
		/// </summary>
		/// <param name="inputDeps">Our input dependencies.</param>
		/// <returns></returns>
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			//We can access MonoBehaviour here, as you can see.
			//Enable this when in Exercise 3 for our tank game, so we dont get errors before that.
//			if (GameSettings.Instance.IsPlayerDead)
//				return inputDeps;

			//Create a new Job, set a value that our query needs and execute what we want.
			//Enable this when in Exercise 3 for our tank game, so we dont get errors before that.
//			var job = new TurnJob
//			{
//				playerPosition = GameSettings.Instance.PlayerPosition
//			};

			//Enable this when in Exercise 3 for our tank game, so we dont get errors before that.
			//Add to our job schedule and we are set.
//			return job.Schedule(this, inputDeps);

			//ERASE THE LINE BELOW when in Exercise 5 for our tank game and enable all lines above.
			return inputDeps;
		}
		// =============================================================================================================
	}
}