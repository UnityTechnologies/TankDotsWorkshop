using UnityEngine;

namespace Workshop.Fundamentals
{
    /// <summary>
    /// Rotate objects normally
    /// </summary>
    public class CubeMove : MonoBehaviour
    {
        public float speed = 1;

        // =============================================================================================================
        private void Update()
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
        // =============================================================================================================
    }
}