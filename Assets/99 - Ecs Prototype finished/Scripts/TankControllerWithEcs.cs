using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System;

namespace MyScripts.TankGame
{
    /// <summary>
    /// Handles our tank movement in-game, this one is using part MonoBehaviour and part ECS.
    /// Hit Button "Fire1" for normal shooting.
    /// Hit Space keyinput to use tank power.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TankControllerWithEcs : MonoBehaviour
    {
        [Header("MECHANICS")]
        public bool useEcs;
        
        [Header("EXTERNAL DATA")]
        [Tooltip("Main game camera to be added as a reference for our turret aim.")]
        public Camera gameCamera;

        [Header("GROUND DETAILS")]
        [Tooltip("Our aim should only consider the ground to aim.")]
        public LayerMask groundLayer;

        [Header("TANK DATA")]
        public float tankMoveSpeed = 10;
        public float tankRotateSpeed = 80;
        [Tooltip("How fast should our turret can rotate.")]
        public float turretRotationSpeed = 50;
        [Tooltip("My turret transform.")]
        public Transform turretTransform;
        [Tooltip("Graphics when my tank is alive.")]
        public GameObject tankNormalGraphics;
        [Tooltip("Graphics when my tank is dead.")]
        public GameObject tankDeadGraphics;
        
        [Header("SHOOT DATA")]
        public Transform tankBarrel;
        public ParticleSystem shotVfx;
        public AudioSource shotAudio;
        public float fireRate = .1f;
        public int spreadAmount = 20;
        public GameObject bulletPrefab;

        [Header("AUDIO")]
        public AudioSource audioEngineIdle;
        public AudioSource audioDriving;
        
        //Cache
        private Rigidbody m_Rigidbody;
        private Transform m_Transform;
        private float m_ShootingTimer;
        
        /// <summary>
        /// Am I dead?
        /// </summary>
        private bool m_IsDead;
        
        // =============================================================================================================
        private void OnEnable()
        {
            GameSettings.OnPlayerDied += OnPlayerIsDead;
        }
        // =============================================================================================================
        private void OnDisable()
        {
            GameSettings.OnPlayerDied -= OnPlayerIsDead;
        }
        // =============================================================================================================
        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Transform = GetComponent<Transform>();
            tankNormalGraphics.SetActive(true);
            tankDeadGraphics.SetActive(false);
            PrepareEcsBullet();
        }
        // =============================================================================================================
        private void Update()
        {
            HandleTurretController();
            HandleShooting();
        }
        // =============================================================================================================
        private void FixedUpdate()
        {
            HandleTankMovement();
        }
        // =============================================================================================================
        private void HandleTurretController()
        {
            if (m_IsDead)
                return;
            //We want the turret to look where our mouse pointer is touching the ground
            var ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, groundLayer))
            {
                Vector3 lookPos = hit.point - m_Transform.position;
                lookPos.y = 0;
                lookPos.Normalize();
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                //Rotate towards our target
                turretTransform.rotation =
                    Quaternion.Slerp(turretTransform.rotation, rotation, Time.deltaTime * turretRotationSpeed);
            }
        }
        // =============================================================================================================
        private void HandleTankMovement()
        {
            if (m_IsDead)
                return;
            //Arrow Key Input
            var inputHorizontal = Input.GetAxis("Horizontal");
            var inputVertical = Input.GetAxis("Vertical");
            
            //Move
            Vector3 movement = inputVertical * Time.fixedDeltaTime * tankMoveSpeed * m_Transform.forward;
            m_Rigidbody.MovePosition(m_Transform.position + movement);
            
            //Rotate
            Quaternion newRotation = Quaternion.Euler(0, Time.fixedDeltaTime * inputHorizontal * tankRotateSpeed, 0);
            newRotation.Normalize();
            m_Rigidbody.MoveRotation(m_Transform.rotation * newRotation);
            
            //Audio
            if (Math.Abs(inputHorizontal) > 0 || Math.Abs(inputVertical) > 0)
            {
                audioEngineIdle.Stop();
                if (!audioDriving.isPlaying) audioDriving.Play();
            }
            else
            {
                if (!audioEngineIdle.isPlaying) audioEngineIdle.Play();
                audioDriving.Stop();
            }
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

            if (useEcs)
            {
                if (usePower)
                    SpawnTankPowerShoot_ECS(rotation);
                else
                    SpawnTankShell_ECS(rotation);
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
        /// <summary>
        /// Called by the GameSettings when the player dies.
        /// </summary>
        private void OnPlayerIsDead()
        {
            m_IsDead = true;
            tankNormalGraphics.SetActive(false);
            tankDeadGraphics.SetActive(true);
            audioEngineIdle.Stop();
            audioDriving.Stop();
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
            if (useEcs)
            {
                m_Manager = World.Active.EntityManager;
                m_BulletEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, World.Active);
            }
        }
        // =============================================================================================================
        private void SpawnTankShell_ECS(Vector3 rotation)
        {
            Entity bullet = m_Manager.Instantiate(m_BulletEntityPrefab);
            m_Manager.SetComponentData(bullet, new Translation { Value = tankBarrel.position });
            m_Manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(rotation) });
        }
        // =============================================================================================================
        private void SpawnTankPowerShoot_ECS(Vector3 rotation)
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