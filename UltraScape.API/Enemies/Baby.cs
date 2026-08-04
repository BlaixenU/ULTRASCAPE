using UnityEngine;

namespace UltraScape.API.Enemies;

public class Baby : MonoBehaviour
{
    public GameObject spriteObject;

	public Animator Animator => spriteObject.GetComponent<Animator>();

	public GameObject lockOnSound;

	public GameObject chargeSound;

	public HurtZone hurtZone;

	[Space]
	public float chargeDistance;

	public float chargeDuration;

	public float ChargeSpeed => chargeDistance / chargeDuration;

	private float chargeStartTime;

	public float TimeSinceChargeStart => Time.realtimeSinceStartup - chargeStartTime;

	public float lockOnDuration;

	private float lockOnStartTime;

	public float TimeSinceLockOnStart => Time.realtimeSinceStartup - lockOnStartTime;

	public float idleDuration;

	private float idleStartTime;

	public float TimeSinceIdleStart => Time.realtimeSinceStartup - idleStartTime;

	private bool transitionIn;

	public BabyState BabyState { get; private set; }
    
    void Start()
    {
        BabyState = BabyState.Idle;

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
	}

	private void EnterCharge()
	{
		SetState(BabyState.Charge);
		Instantiate(chargeSound, gameObject.transform);
	}

	private void EnterIdle()
	{
		SetState(BabyState.Idle);
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

