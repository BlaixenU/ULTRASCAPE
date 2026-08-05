using UnityEngine;

namespace UltraScape.API.Enemies
{
	
	public class Baby : MonoBehaviour
	{
		public Animator Animator => gameObject.GetComponent<Animator>();

		public GameObject lockOnSound;

		public GameObject chargeSound;

		public HurtZone hurtZone;

		[Space]
		public float chargeDistance;

		public float chargeDuration;

		public float ChargeSpeed => chargeDistance / chargeDuration;

		private double chargeStartTime;

		public double TimeSinceChargeStart => Time.realtimeSinceStartupAsDouble - chargeStartTime;

		public double lockOnDuration;

		private double lockOnStartTime;

		public double TimeSinceLockOnStart => Time.realtimeSinceStartupAsDouble - lockOnStartTime;

		public double idleDuration;

		private double idleStartTime;

		public double TimeSinceIdleStart => Time.realtimeSinceStartupAsDouble - idleStartTime;

		private bool transitionIn;

		public BabyState BabyState { get; private set; }

		void Start()
		{
			if (hurtZone == null)
			{
				hurtZone = GetComponent<HurtZone>();
			}

			EnterIdle();

			transitionIn = true;

			SyncState();
			hurtZone.enabled = true;
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

