using UnityEngine;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// Rotate objects normally
    /// </summary>
    public class CubeRotate : MonoBehaviour
    {
        public float speed = 45;

        // =============================================================================================================
        private void Update()
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
        // =============================================================================================================
    }
}