using UnityEngine;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// Just spawn a lot of objects, using normal MonoBehaviour to see the difference later with ECS.
    /// </summary>
    public class SpawnMonoObjects : MonoBehaviour
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
            var id = 0;
            for (var x = 0; x < spawnCubeSize; x++)
            {
                for (var y = 0; y < spawnCubeSize; y++)
                {
                    for (var z = 0; z < spawnCubeSize; z++)
                    {
                        var myPos = transform.position;
                        var pos = new Vector3(myPos.x + spawnSeparation * x,
                            myPos.y + spawnSeparation * y,
                            myPos.z + spawnSeparation * z);
                        var go = Instantiate(spawnPrefab, pos, Quaternion.identity);
                        id++;
                        go.name = "LifeCube_" + id;
                    }
                }
            }
        }
        // =============================================================================================================
    }
}