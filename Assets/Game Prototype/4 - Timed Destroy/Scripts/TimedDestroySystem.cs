using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Workshop.TankGame
{
	/// <summary>
	/// Job System to make objects to die during execution, after some given time.
	/// More info here:
	/// https://docs.unity3d.com/Packages/com.unity.entities@0.1/api/Unity.Entities.EntityCommandBufferSystem.html
	/// </summary>
	public class TimedDestroySystem : JobComponentSystem
	{
		EndSimulationEntityCommandBufferSystem buffer;

		// =============================================================================================================
		protected override void OnCreate()
		{
			buffer = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
		}
		// =============================================================================================================
		/// <summary>
		/// This Job is different, this one can get the Entity id.
		/// It gives us the Entity we are updating and the job index.
		/// We need the job index in order to issue commands.
		/// </summary>
		private struct CullingJob : IJobForEachWithEntity<TimeToLive>
		{
			/// <summary>
			/// Commands can give us different options, like the DestroyEntity, and in a safe way of doing so
			/// inside a Job System.
			/// </summary>
			public EntityCommandBuffer.Concurrent commands;
			public float dt;

			public void Execute(Entity entity, int jobIndex, ref TimeToLive timeToLive)
			{
				timeToLive.lifeTimer -= dt;
				if (timeToLive.lifeTimer <= 0f)
					commands.DestroyEntity(jobIndex, entity);
			}
		}
		// =============================================================================================================
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var job = new CullingJob
			{
				commands = buffer.CreateCommandBuffer().ToConcurrent(),
				dt = Time.deltaTime
			};

			// When creating an EntityCommandBuffer for a Job, we need to pass that job’s handle to the Buffer System.
			// That ensures that the system knows which jobs need to be completed before executing the commands.
			var system = World.Active.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
			
			// We schedule the job and then execute it in a safe order.
			var handle = job.Schedule(this, inputDeps);
			system.AddJobHandleForProducer(handle);

			return handle;
		}
		// =============================================================================================================
	}
}