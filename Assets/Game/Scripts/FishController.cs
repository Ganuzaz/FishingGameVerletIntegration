using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum FishType
{
    v1, v2, v3, v4
}

public enum FishState
{
    Idle,
    RandomSwim,
    SwimToBait,
    Attached,
    Reeled
}


public class FishController : MonoBehaviour
{
    public FishType fishType;

    private Vector3 startPos;
    private Vector3 targetPos;


    private FishState fishState;
    private float passedTime = 0;
    private float targetTime = 0;

    private Quaternion initialRotation;

    private Quaternion startRot;

    private float totalTimeToTarget = 1f;
    private GameObject bait;

    // Start is called before the first frame update
    void Start()
    {
        initialRotation = transform.rotation;
        ChangeState(FishState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        passedTime += Time.deltaTime;

        switch (fishState)
        {
            case FishState.Idle:
                if (passedTime / targetTime >= 1)
                {
                    ChangeState(FishState.RandomSwim);
                }
                break;

            case FishState.RandomSwim:
                SwimToTarget();
                if (passedTime / targetTime >= 1f)
                {
                    ChangeState(FishState.Idle);
                }
                break;

            case FishState.SwimToBait:
                SwimToTarget();
                if (passedTime / targetTime >= 1f)
                {
                    ChangeState(FishState.Attached);
                }

                break;

            case FishState.Attached:
                AttachFish();
                break;

            case FishState.Reeled:
                AttachFish();
                RotateToUp();
                break;

        }
        //Debug.Log($"{fishState} {passedTime}");


    }

    private void SwimToTarget()
    {
        var direction = (targetPos - transform.position).normalized;

        transform.position = Vector3.Lerp(startPos, targetPos, passedTime / targetTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), passedTime / totalTimeToTarget);

    }

    public void StartSwimToBait(GameObject bait)
    {
        totalTimeToTarget = 3f;
        targetPos = bait.transform.position;
        ChangeState(FishState.SwimToBait);
        this.bait = bait;
        EventManager.instance.AddListenerOnce(EventEnums.onPlayerFishingBaitReel, OnPlayerReelFishingRod);

    }

    void OnPlayerReelFishingRod(Hashtable hashtable)
    {
        switch (fishState)
        {
            case FishState.SwimToBait:
                ChangeState(FishState.Idle);
                break;

            case FishState.Attached:
                ChangeState(FishState.Reeled);
                break;
        }
    }


    private void SwimToBait()
    {

    }

    private void AttachFish()
    {
        transform.position = bait.transform.position;
    }

    void RotateToUp()
    {
        Vector3 degrees = new Vector3(-90, 0, 0);
        startRot = transform.rotation;
        Quaternion end = Quaternion.Euler(degrees);

        transform.rotation = Quaternion.Lerp(startRot, end, passedTime);

    }


    private void ResetFish(Hashtable hashtable = null)
    {
        var spawnPos = StageManager.instance.GetRandomBoundaryPos();

        transform.position = spawnPos;
        transform.rotation = initialRotation;
        totalTimeToTarget = 1f;
        ChangeState(FishState.Idle);

    }




    private void ChangeState(FishState fishState)
    {
        if (this.fishState == fishState)
            return;

        switch (fishState)
        {
            case FishState.Idle:
                targetTime = Random.Range(0f, 2f);
                break;
            case FishState.RandomSwim:
                targetPos = StageManager.instance.GetRandomBoundaryPos();
                targetTime = Random.Range(2f, 4f);
                startPos = transform.position;
                break;
            case FishState.Attached:
                EventManager.instance.InvokeEvent(EventEnums.onFishAttached);
                break;
            case FishState.Reeled:
                EventManager.instance.AddListenerOnce(EventEnums.onGameReset, ResetFish);
                EventManager.instance.InvokeEvent(EventEnums.onFishGotReeled, new Hashtable() { {"fishType",fishType } });
                break;
        }

        passedTime = 0;
        this.fishState = fishState;

    }



}
