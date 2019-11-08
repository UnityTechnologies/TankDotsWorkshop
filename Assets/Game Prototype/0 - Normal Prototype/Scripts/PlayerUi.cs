using UnityEngine;

namespace Workshop.TankGame
{
    /// <summary>
    /// Handles player UI, for screen or world space.
    /// </summary>
    public class PlayerUi : MonoBehaviour
    {
        [Header("UI - WORLD SPACE")]
        [Tooltip("Add the Image transform here that will be used to show player ui health.")]
        public RectTransform uiHealthOnScene;

        private float m_UiSize;
        
        // =============================================================================================================
        private void Update()
        {
            m_UiSize = GameSettings.Instance.PlayerCurrentHealth / GameSettings.Instance.PlayerMaxHealth;
            uiHealthOnScene.localScale = new Vector3(m_UiSize, 1, 1);
        }
        // =============================================================================================================
    }
}