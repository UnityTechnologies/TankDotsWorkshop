using UnityEngine;

namespace Workshop.TankGame
{
    /// <summary>
    /// Handles the bullet from the tank. This one is using Unity MonoBehaviour
    /// but pay attention to the points where we will change to DOTS (ECS).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TankBulletBehaviour : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Speed of the bullet to move forward.")]
        public float speed = 50f;

        [Header("Life Settings")]
        [Tooltip("How long should we live, if don't touch anything?")]
        public float lifeTime = 2f;

        [Header("HIT TAGS")]
        public string enemyTag = "Enemy";
        public string environmentTag = "Environment";

        //Cache
        private Rigidbody m_ProjectileRigidbody;
        private Transform m_Transform;

        // =============================================================================================================
        private void Start()
        {
            //Let's get our main component for physics and invoke a function to destroy this object, in case it does
            //not touch anything
            m_ProjectileRigidbody = GetComponent<Rigidbody>();
            m_Transform = GetComponent<Transform>();
            Invoke(nameof(RemoveProjectile), lifeTime);
        }
        // =============================================================================================================
        private void Update()
        {
            //Make it move forward
            Vector3 movement = Time.deltaTime * speed * m_Transform.forward;
            m_ProjectileRigidbody.MovePosition(m_Transform.position + movement);
        }
        // =============================================================================================================
        private void OnTriggerEnter(Collider theCollider)
        {
            if (theCollider.CompareTag(enemyTag))
            {
                GameSettings.Instance.AddScore();
                RemoveProjectile();
                return;
            }
            
            if (theCollider.CompareTag(environmentTag))
                RemoveProjectile();
        }
        // =============================================================================================================
        /// <summary>
        /// Destroy myself.
        /// </summary>
        private void RemoveProjectile()
        {
            Destroy(gameObject);
        }
        // =============================================================================================================
    }
}