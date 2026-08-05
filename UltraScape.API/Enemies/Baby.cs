using UnityEngine;

namespace UltraScape.API.Enemies
{
	
	public class Baby : MonoBehaviour
	{
		public Animator Animator => gameObject.GetComponent<Animator>();

		public GameObject lockOnSound;

		public GameObject chargeSound;

		[Space]
		[SerializeField]
		private AnimationCurve chargeCurve;

		public float chargeDistance;

		public float chargeDuration;

		private double chargeStartTime;

		public double TimeSinceChargeStart => Time.realtimeSinceStartupAsDouble - chargeStartTime;

		public double lockOnDuration;

		private double lockOnStartTime;

		public double TimeSinceLockOnStart => Time.realtimeSinceStartupAsDouble - lockOnStartTime;

		public double idleDuration;

		private double idleStartTime;

		public double TimeSinceIdleStart => Time.realtimeSinceStartupAsDouble - idleStartTime;

		private Vector3 chargeStartPos;

		private Vector3 attackVector;

		public BabyState BabyState { get; private set; }

		void Start()
		{
			EnterIdle();
		}

		private void Update()
		{
			if (!NewMovement.Instance)
			{
				SetState(BabyState.Idle);
				return;
			}

			switch (BabyState)
			{
				case BabyState.Idle:
					{
						if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
						{
							if (TimeSinceIdleStart >= idleDuration)
							{
								EnterLockOn();
							}
						}
						break;
					}
				case BabyState.LockOn:
					if (TimeSinceLockOnStart >= lockOnDuration)
					{
						EnterCharge();
					}
					break;
				case BabyState.Charge:
					if (TimeSinceChargeStart >= chargeDuration)
					{
						EnterIdle();
					}

					transform.position = chargeStartPos + (attackVector * chargeCurve.Evaluate((float)((Time.realtimeSinceStartup - chargeStartTime) / chargeDuration)));

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

		private void SyncState()
		{
			Animator.SetInteger("BabyState", (int)BabyState);
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
			lockOnStartTime = Time.realtimeSinceStartupAsDouble;

			chargeStartPos = transform.position;

			var playerPos = NewMovement.Instance.transform.position;

			attackVector = Quaternion.LookRotation(playerPos - chargeStartPos) * Vector3.forward * chargeDistance;
		}

		private void EnterCharge()
		{
			SetState(BabyState.Charge);
			Instantiate(chargeSound, gameObject.transform);
			chargeStartTime = Time.realtimeSinceStartupAsDouble;
		}

		private void EnterIdle()
		{
			SetState(BabyState.Idle);
			idleStartTime = Time.realtimeSinceStartupAsDouble;
		}

		private void ResetCooldowns()
		{
			chargeStartTime = 0f;
			lockOnStartTime = 0f;
			idleStartTime = 0f;
		}
	}

	public enum BabyState
	{
		Idle,
		LockOn,
		Charge
	}

}

