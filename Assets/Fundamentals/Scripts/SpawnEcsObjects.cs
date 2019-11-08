using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// We will spawn ECS objects here now.
    /// </summary>
    public class SpawnEcsObjects : MonoBehaviour
    {
        [Tooltip("The object we want to spawn.")]
        [SerializeField] private GameObject spawnPrefab;
        [Tooltip("This number will be multiplied 3 times. So, 20, will be 8000 objects created.")]
        [SerializeField] private float spawnCubeSize = 20f;
        [SerializeField] private float spawnSeparation = 2f;
        
        // =============================================================================================================
        private void Start()
        {
            SpawnObjects();
        }
        // =============================================================================================================
        /// <summary>
        /// Spawn objects to test.
        /// </summary>
        private void SpawnObjects()
        {
            //We access the EntityManager of the active world (the default one)
            EntityManager entityManager = World.Active.EntityManager;
        
            //Convert the GameObject prefab to an ECS Entity that works as a prefab
            Entity entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(spawnPrefab, World.Active);

            for (var x = 0; x < spawnCubeSize; x++)
            {
                for (var y = 0; y < spawnCubeSize; y++)
                {
                    for (var z = 0; z < spawnCubeSize; z++)
                    {
                        //Use the manager to instantiate the entity
                        Entity prefabInstance = entityManager.Instantiate(entityPrefab);
                    
                        //Create a Translation component with the desired spawn location, notice...
                        //...that this system uses float3 instead of Vector3
                        Translation instancePosition = new Translation();
                        instancePosition.Value = new float3(x * spawnSeparation, y * spawnSeparation, z * spawnSeparation);
                        //Set the position
                        entityManager.SetComponentData(prefabInstance, instancePosition);
                    }
                }
            }
        }
        // =============================================================================================================
    }
}