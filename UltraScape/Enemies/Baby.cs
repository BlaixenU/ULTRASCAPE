using UnityEngine;

namespace UltraScape.Enemies
{
	public class Baby : MonoBehaviour
	{
		[Header("Asset References")]
		public GameObject lockOnSound = null!;

		public GameObject chargeSound = null!;

		public GameObject trailObject = null!;

		public GameObject lockOnFlashObject = null!;


		private GameObject lockOnFlashInstance = null!;

		private Animator animator = null!;

		private Enemy enemy = null!;

		[Space]
		public AnimationCurve chargeCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

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

		public BabyState BabyState { get; private set; }

		private void Awake()
		{
			animator = GetComponent<Animator>();
			enemy = GetComponent<Enemy>();
		}

		private void Start()
		{
			ResetCooldowns();
			EnterIdle();
		}

		private void Update()
		{
			if (GetComponent<SphereCollider>())
			{
				GetComponent<SphereCollider>().enabled = true;
			}

			if (!NewMovement.Instance)
			{
				EnterIdle();
				return;
			}

			switch (BabyState)
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

		private void FixedUpdate()
		{
		}

		public void SyncState()
		{
			animator.SetInteger("BabyState", (int)BabyState);
		}

		public void SetState(BabyState state)
		{
			BabyState = state;
			SyncState();
		}

		private void EnterLockOn()
		{
			SetState(BabyState.LockOn);
			Instantiate(lockOnSound, gameObject.transform);
			lockOnTimer = 0;

			chargeStartPos = transform.position;

			var playerPos = NewMovement.Instance!.transform.position;

			attackVector = Quaternion.LookRotation(playerPos - chargeStartPos) * Vector3.forward * chargeDistance;

			trailInstance = PrepareTrail(chargeStartPos, chargeStartPos + attackVector);

			lockOnFlashInstance = Instantiate(lockOnFlashObject, gameObject.transform);
			lockOnFlashInstance.transform.localPosition = new(0, 0, 0.1f);
		}

		private void EnterCharge()
		{
			SetState(BabyState.Charge);
			Instantiate(chargeSound, gameObject.transform);
			chargeTimer = 0;

			trailInstance?.StartFade();
		}

		private void EnterIdle()
		{
			SetState(BabyState.Idle);
			idleTimer = 0;
		}

		public void ResetCooldowns()
		{
			chargeTimer = 0;
			idleTimer = 0;
			lockOnTimer = 0;
		}

		private BabyTrail PrepareTrail(Vector3 startPos, Vector3 endPos)
		{
			var trail = Instantiate(trailObject);
			var lr = trail.GetComponent<LineRenderer>();

			lr.SetPosition(0, startPos);
			lr.SetPosition(1, endPos);

			return trail.GetComponent<BabyTrail>();
		}

		private void OnDestroy()
		{
			if (lockOnFlashInstance)
			{
				Destroy(lockOnFlashInstance);
			}
		}
	}

	public enum BabyState
	{
		Idle,
		LockOn,
		Charge
	}

}

