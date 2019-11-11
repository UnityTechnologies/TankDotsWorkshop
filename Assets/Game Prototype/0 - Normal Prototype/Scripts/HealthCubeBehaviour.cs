using UnityEngine;

namespace Workshop.TankGame
{
    public class HealthCubeBehaviour : MonoBehaviour
    {
        [Tooltip("How much health we will restore?")]
        public float healthRestore = 10;
        
        [Header("HIT TAGS")]
        public string playerTag = "Player";

        // =============================================================================================================
        private void OnTriggerEnter(Collider theCollider)
        {
            if (theCollider.CompareTag(playerTag))
            {
                GameSettings.Instance.PlayerDamage(healthRestore * -1); //since we are using the Dmg function
                Destroy(gameObject);
            }
        }
        // =============================================================================================================
    }
}