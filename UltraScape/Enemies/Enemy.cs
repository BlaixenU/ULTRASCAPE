using UnityEngine;
using UnityEngine.Events;

namespace UltraScape.Enemies
{
	public class Enemy : MonoBehaviour
	{
		[Header("Interface with UltraScape.Enemies.EnemyManager")]
		public UnityEvent onCreate = null!;

		public UnityEvent onDestroy = null!;

		private USEnemyType enemyType;

		private EnemyManager eman => EnemyManager.Instance!;

		private void Awake()
		{
			enemyType = GetEnemyType();

			if (enemyType == USEnemyType.none)
			{
				Destroy(this);
			}
		}

		public USEnemyType GetEnemyType()
		{
			if (GetComponent<Baby>())
			{
				return USEnemyType.baby;
			}
			return USEnemyType.none;
		}

		private void Start()
		{
			EnemyManager.Instance!.AddEnemy(gameObject, enemyType);

			onCreate.Invoke();
		}

		private void OnDestroy()
		{
			EnemyManager.Instance!.RemoveEnemy(gameObject, enemyType);

			onDestroy.Invoke();
		}

		public void Instantiate(GameObject targetObject) // helper to show in UnityEvents
		{
			Instantiate(targetObject, gameObject.transform);
		}
	}

	public enum USEnemyType
	{
		none,
		baby // dont add anything until im sure im adding it or i'll feel ashamed and quit working on this
	}
}