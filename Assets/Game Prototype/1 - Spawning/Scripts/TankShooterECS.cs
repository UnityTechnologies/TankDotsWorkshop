using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

namespace Workshop.TankGame
{
    public class TankShooterECS : MonoBehaviour
    {
        [Header("ECS MECHANICS")]
        public bool useECS;
        
        [Header("SHOOT DATA")]
        public Transform tankBarrel;
        public ParticleSystem shotVfx;
        public float fireRate = .1f;
        public int spreadAmount = 20;
        public GameObject bulletPrefab;
        public AudioSource shotAudio;
        
        /// <summary>
        /// Am I dead?
        /// </summary>
        private bool m_IsDead;
        
        //Cache
        private float m_ShootingTimer;
        
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
            PrepareEcsBullet();
        }
        // =============================================================================================================
        private void Update()
        {
            HandleShooting();
        }
        // =============================================================================================================
        /// <summary>
        /// Called by the GameSettings when the player dies.
        /// </summary>
        private void OnPlayerIsDead()
        {
            m_IsDead = true;
        }
        // =============================================================================================================
        /// <summary>
        /// Handles the shooting mechanic, pressing "Fire1" or Space key.
        /// </summary>
        private void HandleShooting()
        {
            if (m_IsDead)
                return;
            
            m_ShootingTimer += Time.deltaTime;
            if (Input.GetButton("Fire1") && m_ShootingTimer >= fireRate)
            {
                ShootShell(false);
            }
            if (Input.GetKeyDown(KeyCode.Space) && m_ShootingTimer >= fireRate)
            {
                ShootShell(true);
            }
        }
        // =============================================================================================================
        /// <summary>
        /// Shoot our shells, it can be using our power or normal shooting.
        /// </summary>
        /// <param name="usePower">Use our power for shooting or normal shooting?</param>
        private void ShootShell(bool usePower)
        {
            var rotation = tankBarrel.rotation.eulerAngles;
            rotation.x = 0f;

            if (useECS)
            {
                if (usePower)
                    SpawnTankPowerShootECS(rotation);
                else
                    SpawnTankShellECS(rotation);
            }
            else
            {
                if (usePower)
                    SpawnTankPowerShoot(rotation);
                else
                    SpawnTankShell(rotation);
            }
            
            m_ShootingTimer = 0f;
            if (shotVfx)
                shotVfx.Play();

            if (shotAudio)
                shotAudio.Play();
        }
        // =============================================================================================================
        /// <summary>
        /// Spawn our tank shell (bullet) using normal MonoBehaviour.
        /// </summary>
        /// <param name="rotation"></param>
       private void SpawnTankShell(Vector3 rotation)
        {
            var bullet = Instantiate(bulletPrefab);
            bullet.transform.position = tankBarrel.position;
            bullet.transform.rotation = Quaternion.Euler(rotation);
        }
        // =============================================================================================================
        /// <summary>
        /// Spawn our tank special power (lots of bullets) using normal MonoBehaviour.
        /// </summary>
        /// <param name="rotation"></param>
        private void SpawnTankPowerShoot(Vector3 rotation)
        {
            int max = spreadAmount / 2;
            int min = -max;
            Vector3 tempRot = rotation;
            for (int x = min; x < max; x++)
            {
                tempRot.x = (rotation.x + 3 * x) % 360;
                for (int y = min; y < max; y++)
                {
                    tempRot.y = (rotation.y + 3 * y) % 360;
                    GameObject bullet = Instantiate(bulletPrefab);
                    bullet.transform.position = tankBarrel.position;
                    bullet.transform.rotation = Quaternion.Euler(tempRot);
                }
            }
        }
        // =============================================================================================================
        
        #region ECS
        
        private EntityManager m_Manager;
        private Entity m_BulletEntityPrefab;
        
        // =============================================================================================================
        /// <summary>
        /// If we will use ECS to spawn bullets or shells, let's setup it first. Call this before calling anything
        /// related to our ECS stuff.
        /// </summary>
        private void PrepareEcsBullet()
        {
            if (useECS)
            {
                m_Manager = World.Active.EntityManager;
                m_BulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, World.Active);
            }
        }
        // =============================================================================================================
        private void SpawnTankShellECS(Vector3 rotation)
        {
            Entity bullet = m_Manager.Instantiate(m_BulletEntityPrefab);
            m_Manager.SetComponentData(bullet, new Translation { Value = tankBarrel.position });
            m_Manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(rotation) });
        }
        // =============================================================================================================
        private void SpawnTankPowerShootECS(Vector3 rotation)
        {
            int max = spreadAmount / 2;
            int min = -max;
            int totalAmount = spreadAmount * spreadAmount;
		
            Vector3 tempRot = rotation;
            int index = 0;

            NativeArray<Entity> bullets = new NativeArray<Entity>(totalAmount, Allocator.TempJob);
            m_Manager.Instantiate(m_BulletEntityPrefab, bullets);

            for (int x = min; x < max; x++)
            {
                tempRot.x = (rotation.x + 3 * x) % 360;

                for (int y = min; y < max; y++)
                {
                    tempRot.y = (rotation.y + 3 * y) % 360;
                    m_Manager.SetComponentData(bullets[index], new Translation { Value = tankBarrel.position });
                    m_Manager.SetComponentData(bullets[index], new Rotation { Value = Quaternion.Euler(tempRot) });
                    index++;
                }
            }
            bullets.Dispose();
        }
        // =============================================================================================================
        #endregion
    }
}