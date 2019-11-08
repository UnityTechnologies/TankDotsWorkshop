using UnityEngine;

namespace MyScripts.TankGame
{
    /// <summary>
    /// Spawn our enemies in the scene, using MonoBehaviour only here.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("SPAWN SETUP")]
        public bool spawnEnemies = true;
        public float enemySpawnRadius = 10f;
        public GameObject enemyPrefab;
        [Range(1, 100)] public int spawnsPerInterval = 1;
        [Range(.1f, 2f)] public float spawnInterval = 1f;

        //Cache
        private float m_Cooldown;

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
        private void Spawn()
        {
            for (var i = 0; i < spawnsPerInterval; i++)
            {
                Vector3 pos = GameSettings.Instance.GetPositionAroundPlayer(enemySpawnRadius);
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
        }
        // =============================================================================================================
    }
}