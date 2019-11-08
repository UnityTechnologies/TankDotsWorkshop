using System;
using UnityEngine;

namespace Workshop.TankGame
{
    /// <summary>
    /// Handles our tank movement in-game (this is the Player), this one is using all normal Unity MonoBehaviour.
    /// Hit Button "Fire1" for normal shooting.
    /// Hit Space keyinput to use tank power.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TankController : MonoBehaviour
    {
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

        [Header("AUDIO")]
        public AudioSource audioEngineIdle;
        public AudioSource audioDriving;
        
        //Cache
        private Rigidbody m_Rigidbody;
        private Transform m_Transform;
        
        /// <summary>
        /// Am I dead?
        /// </summary>
        private bool m_IsDead;
        
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
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Transform = GetComponent<Transform>();
            tankNormalGraphics.SetActive(true);
            tankDeadGraphics.SetActive(false);
        }
        // =============================================================================================================
        private void Update()
        {
            HandleTurretController();
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
    }
}