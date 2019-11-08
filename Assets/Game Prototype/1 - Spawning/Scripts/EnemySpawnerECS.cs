using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Workshop.TankGame
{
    /// <summary>
    /// Spawn our enemies in the scene, now changing to ECS.
    /// </summary>
    public class EnemySpawnerECS : MonoBehaviour
    {
        [Header("ECS MECHANICS")]
        public bool useECS;
        
        [Header("SPAWN SETUP")]
        public bool spawnEnemies = true;
        public float enemySpawnRadius = 10f;
        public GameObject enemyPrefab;
        [Range(1, 100)] public int spawnsPerInterval = 1;
        [Range(.1f, 2f)] public float spawnInterval = 1f;

        //Cache
        private float m_Cooldown;
        
        private EntityManager m_Manager;
        private Entity m_EnemyEntityPrefab;

        // =============================================================================================================
        private void Start()
        {
            if (useECS)
            {
                m_Manager = World.Active.EntityManager;
                m_EnemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, World.Active);
            }
        }
        // =============================================================================================================
        private void Update()
        {
            if (!spawnEnemies || GameSettings.Instance.IsPlayerDead)
                return;

            m_Cooldown -= Time.deltaTime;

            if (m_Cooldown <= 0f)
            {
                m_Cooldown += spawnInterval;
                Spawn();
            }
        }
        // =============================================================================================================
        /// <summary>
        /// Spawn a new enemy on map based on the given position, but now converting it to ECS.
        /// </summary>
        private void Spawn()
        {
            for (var i = 0; i < spawnsPerInterval; i++)
            {
                Vector3 pos = GameSettings.Instance.GetPositionAroundPlayer(enemySpawnRadius);
                if (!useECS)
                {
                    Instantiate(enemyPrefab, pos, Quaternion.identity);
                }
                else
                {
                    //Here we spawn the enemy and set the position, called Translation on ECS.
                    Entity enemy = m_Manager.Instantiate(m_EnemyEntityPrefab);
                    m_Manager.SetComponentData(enemy, new Translation { Value = pos });
                }
            }
        }
        // =============================================================================================================
    }
}