using UnityEngine;

namespace UltraScape.API.Enemies
{
	public class Baby : MonoBehaviour
	{
		public GameObject? lockOnSound;

		public GameObject? chargeSound;

		public GameObject? trailObject;

		private Animator? animator;

		[Space]
		[SerializeField]
		private AnimationCurve? chargeCurve;

		[Header("Cooldowns")]
		public float chargeDistance = 50;

		public float chargeDuration = 1.5f;

		private float chargeTimer;

		public float lockOnDuration = 0.75f;

		private float lockOnTimer;

		public float idleDuration = 0.1f;

		private float idleTimer;

		private Vector3 chargeStartPos;

		private Vector3 attackVector;

		private BabyTrail trailInstance; // most of the time null

		public BabyState BabyState { get; private set; }

		void Awake()
		{
			animator = GetComponent<Animator>();
		}
		


		void Start()
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

		private void SetState(BabyState state)
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

			var playerPos = NewMovement.Instance.transform.position;

			attackVector = Quaternion.LookRotation(playerPos - chargeStartPos) * Vector3.forward * chargeDistance;

			trailInstance = PrepareTrail(chargeStartPos, chargeStartPos + attackVector);


		}

		private void EnterCharge()
		{
			SetState(BabyState.Charge);
			Instantiate(chargeSound, gameObject.transform);
			chargeTimer = 0;

			if (trailInstance != null)
			{
				trailInstance.State = TrailState.fade;
			}
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

		public BabyTrail PrepareTrail(Vector3 startPos, Vector3 endPos)
		{
			var trail = Instantiate(trailObject);
			var lr = trail.GetComponent<LineRenderer>();
			
			lr.SetPosition(0, startPos);
			lr.SetPosition(1, endPos);

			return trail.GetComponent<BabyTrail>();
		}
	}

	public enum BabyState
	{
		Idle,
		LockOn,
		Charge
	}

}

