using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

//Many of the game ideas here, and part of the code, came from Mike Geig's AngryDOTS project, thank you =)
namespace Workshop.TankGame
{
    /// <summary>
    /// Here we have:
    /// - All settings for our game.
    /// - All player data, regarding health, score, etc.
    /// It does not need to have any ECS related stuff, because we don't need it here.
    /// </summary>
    public class GameSettings : MonoBehaviour
    {
        /// <summary>
        /// We should access our game settings from any script
        /// </summary>
        public static GameSettings Instance { get; private set; }

        [Header("GAME SETUP")]
        [Tooltip("My game score.")]
        [SerializeField] private int playerScore;
        [Tooltip("Amount of points to add to your score per enemy killed.")]
        [SerializeField] private int scorePerEnemy = 1;
        
        [Header("UI SETUP")]
        [SerializeField] private TextMeshProUGUI uiScoreText;
        
        [Header("PLAYER INFORMATION")]
        [Tooltip("Our current player transform, is our case, the Tank.")]
        [SerializeField] private Transform playerTransform;
        [Tooltip("Max player's life.")]
        [SerializeField] private float playerMaxLife = 100;

        [Header("PLAYER INFORMATION")]
        public AudioSource sfxCollectHealth;
        
        /// <summary>
        /// Current player's life.
        /// </summary>
        private float m_PlayerCurrentLife;

        /// <summary>
        /// Action to be called when the player dies, so we can deal with different behaviours.
        /// </summary>
        public static Action onPlayerDied;
        
        // =============================================================================================================
        /// <summary>
        /// Max health of our player.
        /// </summary>
        public float PlayerMaxHealth => playerMaxLife;
        // =============================================================================================================
        /// <summary>
        /// If player's life is equal or below zero, he/she is dead.
        /// </summary>
        public float PlayerCurrentHealth => m_PlayerCurrentLife;
        // =============================================================================================================
        /// <summary>
        /// If player's life is equal or below zero, he/she is dead.
        /// </summary>
        public bool IsPlayerDead => m_PlayerCurrentLife <= 0;
        // =============================================================================================================
        /// <summary>
        /// Get current player position.
        /// </summary>
        public Vector3 PlayerPosition => playerTransform.position;
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
            playerScore = 0;
            uiScoreText.text = playerScore.ToString();
            m_PlayerCurrentLife = playerMaxLife;
        }
        // =============================================================================================================
        /// <summary>
        /// Call it when the player takes a damage from an enemy or something.
        /// Also can be used to restore Health.
        /// </summary>
        public void PlayerDamage(float dmgAmount)
        {
            if (m_PlayerCurrentLife <= 0)
                return;
            //Play something different then dmg, for collecting health
            if (dmgAmount < 0)
                sfxCollectHealth.Play();
            //Take dmg
            m_PlayerCurrentLife -= dmgAmount;
            if (m_PlayerCurrentLife > playerMaxLife)
                m_PlayerCurrentLife = playerMaxLife;
            //If player dies, lets warn all scripts that needs it.
            if (m_PlayerCurrentLife <= 0)
            {
                PlayerDied();
            }
        }
        // =============================================================================================================
        /// <summary>
        /// Kills our player and lets everyone know it.
        /// </summary>
        public void PlayerDied()
        {
            m_PlayerCurrentLife = 0;
            onPlayerDied?.Invoke();
        }
        // =============================================================================================================
        /// <summary>
        /// Get a random position around player's position.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public Vector3 GetPositionAroundPlayer(float radius)
        {
            Vector3 playerPos = playerTransform.position;
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float s = Mathf.Sin(angle);
            float c = Mathf.Cos(angle);
            return new Vector3(c * radius, 1.1f, s * radius) + playerPos;
        }
        // =============================================================================================================
        /// <summary>
        /// Add score to our player
        /// </summary>
        public void AddScore()
        {
            playerScore += scorePerEnemy;
            uiScoreText.text = playerScore.ToString();
        }
        // =============================================================================================================
        
        #region FOR ECS LATER
        
        [Header("DATA FOR ECS ONLY")]
        [Tooltip("Our enemy radius to be used in our custom Collision system. Will be used for ECS only.")]
        [SerializeField] private float enemyCollisionRadius = .5f;
        [Tooltip("Our player radius to be used in our custom Collision system. Will be used for ECS only.")]
        [SerializeField] private float playerCollisionRadius = .2f;
        
        // =============================================================================================================
        /// <summary>
        /// Get our collision radius for our enemies.
        /// </summary>
        public float EnemyCollisionRadius => enemyCollisionRadius;
        // =============================================================================================================
        /// <summary>
        /// Get our collision radius for our player.
        /// </summary>
        public float PlayerCollisionRadius => playerCollisionRadius;
        // =============================================================================================================

        #endregion

    }

}
