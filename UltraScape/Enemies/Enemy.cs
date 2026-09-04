using UnityEngine;
using UnityEngine.Events;

namespace UltraScape.Enemies
{
	public class Enemy : MonoBehaviour // make parent class of enemies, rather than parallel components
	{
		public EnemyScriptableObject enemyScriptableObject = null!;

		[Space(10f)]
		[Header("Triggers")]
		public Collider hurtBox = null!;
		public Collider parryBox = null!;

		private ParryReceiver parryRec = null!;

		[Space(10f)]
		[Header("Events")]
		public UnityEvent onCreate = null!;
		public UnityEvent onDestroy = null!;

		private EnemyManager eman => EnemyManager.Instance!;

		protected virtual void Awake()
		{
			if (parryBox)
			{
				parryRec = parryBox.GetComponent<ParryReceiver>();
			}

			gameObject.transform.SetParent(eman.transform);
		}

		protected virtual void Start()
		{
			EnemyManager.Instance?.AddEnemy(gameObject, enemyScriptableObject);

			onCreate.Invoke();
		}

		protected virtual void OnDestroy()
		{
			EnemyManager.Instance?.RemoveEnemy(gameObject, enemyScriptableObject);

			onDestroy.Invoke();
		}
	}
}