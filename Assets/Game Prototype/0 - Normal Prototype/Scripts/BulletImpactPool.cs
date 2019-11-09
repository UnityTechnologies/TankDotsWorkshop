using UnityEngine;

namespace Workshop.TankGame
{
	/// <summary>
	/// Creates a pooling system for bullet/shells impact.
	/// </summary>
	public class BulletImpactPool : MonoBehaviour
	{
		public static BulletImpactPool Instance { get; private set; }

		public GameObject bulletHitPrefab;

		[Tooltip("How many objects should we instantiate to keep inside the pool.")]
		public int poolSize = 100;

		//Cache
		private GameObject[] m_ImpactPool;
		private int m_CurrentPoolIndex;

		// =============================================================================================================
		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			m_ImpactPool = new GameObject[poolSize];
			for (int i = 0; i < poolSize; i++)
			{
				m_ImpactPool[i] = Instantiate(bulletHitPrefab, transform);
				m_ImpactPool[i].SetActive(false);
			}
		}

		// =============================================================================================================
		/// <summary>
		/// Play VFX from the given position.
		/// </summary>
		/// <param name="position"></param>
		public void PlayBulletImpact(Vector3 position)
		{
			if (++m_CurrentPoolIndex >= m_ImpactPool.Length)
				m_CurrentPoolIndex = 0;

			m_ImpactPool[m_CurrentPoolIndex].SetActive(false);
			m_ImpactPool[m_CurrentPoolIndex].transform.position = position;
			m_ImpactPool[m_CurrentPoolIndex].SetActive(true);
		}
		// =============================================================================================================
	}
}
