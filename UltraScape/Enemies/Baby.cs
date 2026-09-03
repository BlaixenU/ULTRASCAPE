using UnityEngine;

namespace UltraScape.Enemies
{
	public enum BabyState
	{
		Idle,
		LockOn,
		Charge
	}

	public class Baby : Enemy
	{
		[Header("Asset References")]
		public GameObject lockOnSound = null!;

		public GameObject chargeSound = null!;

		[Space(5f)]
		public GameObject trailObject = null!;

		public GameObject lockOnFlashObject = null!;

		[Space(5f)]
		public GameObject normalEffects = null!;

		public GameObject chargeEffects = null!;

		private GameObject lockOnFlashInstance = null!;

		private Animator animator = null!;

		private Enemy enemy = null!;

		[Space(10f)]
		public AnimationCurve chargeCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

		[Space(10f)]
		[Header("Cooldowns")]
		public float chargeDistance = 50;

		public float chargeDuration = 1f;

		private float chargeTimer;

		public float lockOnDuration = 0.75f;

		private float lockOnTimer;

		public float idleDuration = 0.5f;

		private float idleTimer;

		private Vector3 chargeStartPos;

		private Vector3 attackVector;

		private BabyTrail trailInstance = null!; // most of the time null

		public BabyState State { get; private set; }

		protected override void Awake()
		{
			base.Awake();

			animator = GetComponent<Animator>();
			enemy = GetComponent<Enemy>();

			ResetCooldowns();
			EnterIdle();
		}

		protected override void Start()
		{
			base.Start();

			enemy.parryBox.enabled = false;

			normalEffects.SetActive(true);
			chargeEffects.SetActive(false);
		}

		private void Update()
		{
			if (!NewMovement.Instance)
			{
				EnterIdle();
				return;
			}

			switch (State)
			{
				case BabyState.Idle:

					if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && idleTimer >= idleDuration)
					{
						EnterLockOn();
					}

					idleTimer += Time.deltaTime * Time.timeScale;

					break;

				case BabyState.LockOn:

					if (lockOnTimer >= lockOnDuration)
					{
						EnterCharge();
					}

					lockOnTimer += Time.deltaTime * Time.timeScale;

					if ((lockOnTimer > 2 * Time.deltaTime) && lockOnFlashInstance)
					{
						lockOnFlashInstance.transform.localPosition = new(0, 0, -0.01f);
					}


					break;

				case BabyState.Charge:

					if (chargeTimer >= chargeDuration)
					{
						EnterIdle();
					}

					chargeTimer += Time.deltaTime * Time.timeScale;


					var distanceFactorFromCurve = chargeCurve.Evaluate(chargeTimer / chargeDuration);

					transform.position = chargeStartPos + (attackVector * distanceFactorFromCurve);

					break;

				default:

					SetState(BabyState.Idle);
					ResetCooldowns();
					break;

			}
		}

		public void SyncState()
		{
			animator.SetInteger("BabyState", (int)State);
		}

		private void SetState(BabyState _state)
		{
			State = _state;
			SyncState();
		}

		public void EnterLockOn()
		{
			SetState(BabyState.LockOn);
			Instantiate(lockOnSound, gameObject.transform);
			lockOnTimer = 0;

			chargeStartPos = transform.position;

			var playerPos = NewMovement.Instance!.transform.position;

			// attackVector = Quaternion.LookRotation(playerPos - chargeStartPos) * Vector3.forward * chargeDistance;
			attackVector = (playerPos - chargeStartPos).normalized * chargeDistance;

			trailInstance = PrepareTrail(chargeStartPos, chargeStartPos + attackVector);

			lockOnFlashInstance = Instantiate(lockOnFlashObject, gameObject.transform);
			lockOnFlashInstance.transform.localPosition = new(0, 0, 0.1f);
		}

		public void EnterCharge()
		{
			SetState(BabyState.Charge);
			Instantiate(chargeSound, gameObject.transform);
			chargeTimer = 0;

			trailInstance?.StartFade();

			enemy.parryBox.enabled = true;

			normalEffects.SetActive(false);
			chargeEffects.SetActive(true);
		}

		public void EnterIdle()
		{
			SetState(BabyState.Idle);
			idleTimer = 0;

			enemy.parryBox.enabled = false;

			normalEffects.SetActive(true);
			chargeEffects.SetActive(false);
		}

		public void ResetCooldowns()
		{
			chargeTimer = 0;
			idleTimer = 0;
			lockOnTimer = 0;
		}

		private BabyTrail PrepareTrail(Vector3 startPos, Vector3 endPos)
		{
			GameObject trail = Instantiate(trailObject);
			var lr = trail.GetComponent<LineRenderer>();

			lr.SetPosition(0, startPos);
			lr.SetPosition(1, endPos);

			return trail.GetComponent<BabyTrail>();
		}

		public GameObject EndlessSpawn()
		{
			return Instantiate(EnemyManager.enemies[base.enemyType].prefab, EnemyManager.Instance!.transform);
		}
	}
}

