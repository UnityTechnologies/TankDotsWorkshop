using UnityEngine;

namespace Workshop.TankGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyController : MonoBehaviour
    {
        [Header("ENEMY INFO")]
        public float speed = 2f;
        public float enemyHealth = 1f;
        [Tooltip("How much damage I will apply to the player when I touch?")]
        public float damagePower = 10f;
        
        [Header("HIT TAGS")]
        public string bulletTag = "Bullet";
        public string playerTag = "Player";

        [Header("LOOT")]
        [Tooltip("When the enemy dies, it has a chance to spawn this on the ground.")]
        public GameObject lifeCubePrefab;
        [Tooltip("Chance to drop between 0 and 1, where 1 is 100%.")]
        [Range(0f, 1f)] public float chanceToDrop = 0.1f;

        //Cache
        private Rigidbody m_RigidBody;
        private Transform m_Transform;
        private bool m_FollowPlayer;
        
        // =============================================================================================================
        private void OnEnable()
        {
            GameSettings.onPlayerDied += OnPlayerIsDead;
        }
        // =============================================================================================================
        private void OnDisable()
        {
            GameSettings.onPlayerDied -= OnPlayerIsDead;
        }
        // =============================================================================================================
        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Transform = GetComponent<Transform>();
            m_FollowPlayer = true;
        }
        // =============================================================================================================
        private void Update()
        {
            HandleMovement();
        }
        // =============================================================================================================
        private void OnTriggerEnter(Collider theCollider)
        {
            if (theCollider.CompareTag(bulletTag))
            {
                if (--enemyHealth <= 0)
                    DestroyMyself();
                return;
            }
            
            if (theCollider.CompareTag(playerTag))
            {
                GameSettings.Instance.PlayerDamage(damagePower);
                DestroyMyself();
            }
        }
        // =============================================================================================================
        /// <summary>
        /// Handles my enemy movement to my target.
        /// </summary>
        private void HandleMovement()
        {
            if (!m_FollowPlayer)
                return;
            //Rotate to target
            Vector3 heading = GameSettings.Instance.PlayerPosition - m_Transform.position;
            heading.y = 0f;
            m_Transform.rotation = Quaternion.LookRotation(heading);

            //Move forward
            Vector3 movement = Time.deltaTime * speed * m_Transform.forward;
            m_RigidBody.MovePosition(m_Transform.position + movement);
        }
        // =============================================================================================================
        /// <summary>
        /// Kill this object.
        /// </summary>
        private void DestroyMyself()
        {
            BulletImpactPool.Instance.PlayBulletImpact(m_Transform.position);
            //Check if we will drop anything?
            if (Random.Range(0f, 1f) <= chanceToDrop)
            {
                Instantiate(lifeCubePrefab, m_Transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        // =============================================================================================================
        /// <summary>
        /// Called by the GameSettings when the player dies.
        /// </summary>
        private void OnPlayerIsDead()
        {
            m_FollowPlayer = false;
        }
        // =============================================================================================================
        /*public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
        {
            manager.AddComponent(entity, typeof(EnemyTag));
            manager.AddComponent(entity, typeof(MoveForward));
    
            MoveSpeed moveSpeed = new MoveSpeed { Value = speed };
            manager.AddComponentData(entity, moveSpeed);
    
            Health health = new Health { Value = enemyHealth };
            manager.AddComponentData(entity, health);
        }*/
        // =============================================================================================================
    }
}