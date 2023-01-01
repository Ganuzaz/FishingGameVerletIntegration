using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private PlayerState playerState;

    float prevSpeed;

    Quaternion startRotation;
    Vector3 startPosition;

    private enum PlayerState
    {
        Idle,
        Charging,
        Throwing,
        Fishing,
        Reeling
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        startRotation = transform.rotation;
        startPosition = transform.position;
    }

    public void Start()
    {
        InitPlayer();
    }


    void InitPlayer()
    {
        playerState = PlayerState.Idle;
        EventManager.instance.AddListener(EventEnums.onBaitReachTarget, OnBaitReachTarget);
        EventManager.instance.AddListener(EventEnums.onBaitReelFinished, (Hashtable hashtable) => { if (playerState == PlayerState.Reeling) playerState = PlayerState.Idle; ResetPlayerPos(); });
    }

    void ResetPlayerPos()
    {
        transform.rotation = startRotation;
        transform.position = startPosition;
    }

    void Update()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                CheckForCastInput();
                break;
            case PlayerState.Fishing:
                CheckForReelInput();
                break;

            case PlayerState.Charging:
                CheckForChargeInput();
                break;
            case PlayerState.Throwing:

                break;
        }


    }

    private void CheckForCastInput()
    {
        if (GameManager.instance.CanReceiveInput() && Input.GetMouseButtonDown(0))
        {
            animator.SetBool("Cast", true);
            animator.SetBool("Idle", true);
            EventManager.instance.InvokeEvent(EventEnums.onPlayerFishingStartCast);
            playerState = PlayerState.Charging;
        }
    }

    private void CheckForReelInput()
    {
        if (GameManager.instance.CanReceiveInput() && Input.GetMouseButtonDown(0))
        {
            animator.SetBool("Reel", true);
            playerState = PlayerState.Reeling;
        }
    }

    private void CheckForChargeInput()
    {
        if (GameManager.instance.CanReceiveInput() && !Input.GetMouseButton(0))
        {
            EventManager.instance.InvokeEvent(EventEnums.onPlayerChargeRelease);
            playerState = PlayerState.Throwing;
        }

    }


    private void OnBaitReachTarget(Hashtable hashtable)
    {
        playerState = PlayerState.Fishing;
    }




    #region AnimationEvents

    public void OnHoldCastAnimEvent()
    {
        if (playerState == PlayerState.Charging)
        {
            prevSpeed = animator.speed;
            animator.speed = 0;
            EventManager.instance.AddListenerOnce(EventEnums.onPlayerChargeRelease, (Hashtable hashtable) => { animator.speed = prevSpeed; });
        }
    }

    public void OnThrowCastAnimEvent()
    {
        if (playerState == PlayerState.Throwing)
        {
            var targetPos = StageManager.instance.GetThrowTargetPos(UIManager.instance.chargeSlider.value);
            var hashTable = new Hashtable();
            hashTable.Add("targetPos", targetPos);
            EventManager.instance.InvokeEvent(EventEnums.onPlayerFishingBaitThrow,hashTable);
        }
    }

    public void OnReelAnimEvent()
    {
        if (playerState == PlayerState.Reeling)
        {
            EventManager.instance.InvokeEvent(EventEnums.onPlayerFishingBaitReel);
        }
    }

    #endregion


}
