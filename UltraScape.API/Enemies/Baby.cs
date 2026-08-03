using UnityEngine;

namespace UltraScape.API.Enemies;

public class Baby : MonoBehaviour
{
    public GameObject spriteObject;

    public GameObject lockOnSound;

    public GameObject chargeSound;

    public HurtZone hurtZone;

    [Space]
    public float chargeDistance; // in Unity units

    public float chargeTime; // 1 equal chargeDistance covered in 1 second
    // currently the charge anim is loop but it dont look good, probably gonna split charge anim into two

    private Time? chargeStart;

    public float lockOnTime; 

    private Time? lockonStart;

    public float ChargeSpeed => chargeDistance / chargeTime;

    public BabyState BabyState{ get; private set; }

    private bool transitionIn;

    // add runtime methods
    
    void Start()
    {
        BabyState = BabyState.Idle;
        transitionIn = true;

        SyncState();

        hurtZone.enabled = true;
    }

    void Update()
    {
        if (!NewMovement.Instance)
        {
            SetState(BabyState.Idle);
            return;
        }

        if (BabyState == BabyState.Idle)
        {
            
        }
    }

    void FixedUpdate()
    {
        
    }

    void SyncState()
    {
        spriteObject.GetComponent<Animator>().SetInteger("BabyState", (int)BabyState);
    }

    void SetState(BabyState state) // primary state setter, use SyncState() to correct state desyncs between class and animator
    {
        BabyState = state;
        SyncState();
    }
}

public enum BabyState
{
    Idle,
    LockOn,
    Charge
}