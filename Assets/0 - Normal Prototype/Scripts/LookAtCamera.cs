using UnityEngine;

namespace MyScripts.TankGame
{
    /// <summary>
    /// Utility script, that will make the object on this script to always face camera.
    /// </summary>
    public class LookAtCamera : MonoBehaviour
    {
        [Tooltip("Target to look at, always.")]
        public Transform cameraTarget;
        private Transform m_Transform;
        
        // =============================================================================================================
        private void Start()
        {
            m_Transform = GetComponent<Transform>();
        }
        // =============================================================================================================
        private void Update()
        {
            m_Transform.LookAt(cameraTarget);
        }
        // =============================================================================================================
    }
}