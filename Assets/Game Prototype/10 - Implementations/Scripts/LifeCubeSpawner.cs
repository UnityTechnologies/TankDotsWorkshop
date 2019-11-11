using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Workshop.TankGame
{
    /// <summary>
    /// Spawn our enemies in the scene, now changing the spawn option to ECS.
    /// </summary>
    public class LifeCubeSpawner : MonoBehaviour
    {
        /// <summary>
        /// We should access our game settings from any script
        /// </summary>
        public static LifeCubeSpawner Instance { get; private set; }
        
        [Header("SPAWN SETUP")]
        [Tooltip("Chance to spawn, between 1 and 100 %")]
        [Range(1, 100)] public int chanceToSpawn = 30;
        [SerializeField] private GameObject lifeCubePrefab;
        
        private EntityManager m_Manager;
        private Entity m_LifeCubeEntityPrefab;
        
        // =============================================================================================================
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        // =============================================================================================================
        private void Start()
        {
            m_Manager = World.Active.EntityManager;
            m_LifeCubeEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(lifeCubePrefab, World.Active);
        }
        // =============================================================================================================
        /// <summary>
        /// Spawn a Life Cube using ECS, but there is a chance to do it.
        /// </summary>
        public void SpawnLifeCubeECS(Vector3 pos)
        {
            if (Random.Range(1, 100) > chanceToSpawn)
                return;
            Entity spawnObj = m_Manager.Instantiate(m_LifeCubeEntityPrefab);
            m_Manager.SetComponentData(spawnObj, new Translation { Value = pos });
        }
        // =============================================================================================================
    }
}