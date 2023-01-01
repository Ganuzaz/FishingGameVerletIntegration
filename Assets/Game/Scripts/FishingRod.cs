using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{

    public GameObject tipPointObject;
    public GameObject bait;

    private float baitSpeed = 7f;

    private enum RodState
    {
        Idle,
        Fishing,
        Reeling,
        Throwing
    }

    private RodState currentRodState;

    private Vector3 currentBaitTargetPos = Vector3.zero;
    private Vector3 baitStartPos = Vector3.zero;

    private float timeToTargetPos = 0;
    private float passedTime = 0;

    public GameObject anchor;
    public ParticleSystem waterEffect;
    float arcHeight = 3f;

    // Start is called before the first frame update
    void Start()
    {
        var currentPos = tipPointObject.transform.position;
        currentRodState = RodState.Idle;

        InitEvents();
    }

    void InitEvents()
    {
        EventManager.instance.AddListener(EventEnums.onPlayerFishingBaitThrow, OnPlayerFishingThrow);
        EventManager.instance.AddListener(EventEnums.onPlayerFishingBaitReel, OnPlayerReel);
        EventManager.instance.AddListener(EventEnums.onFishAttached, (Hashtable hashtable) => { waterEffect.transform.SetParent(null); waterEffect.transform.position = bait.transform.position; waterEffect.Play(); });
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentRodState)
        {
            case RodState.Throwing:
                MoveBaitToTargetPos();
                break;

            case RodState.Reeling:
                MoveToRod();
                break;
        }

    }

    private void MoveBaitToTargetPos()
    {
        passedTime += Time.deltaTime;

        if (passedTime >= timeToTargetPos)
        {
            bait.transform.position = currentBaitTargetPos;
            currentRodState = RodState.Fishing;
            EventManager.instance.InvokeEvent(EventEnums.onBaitReachTarget, new Hashtable() { { "bait", bait } });
            return;
        }

        var t = (passedTime / timeToTargetPos);
        var nextPos = Vector3.Lerp(baitStartPos, currentBaitTargetPos, t);


        float dist = currentBaitTargetPos.x - baitStartPos.x;
        float arc = arcHeight * (nextPos.x - baitStartPos.x) * (nextPos.x - currentBaitTargetPos.x) / (-0.25f * dist * dist);

        nextPos = new Vector3(nextPos.x, nextPos.y + arc, nextPos.z);

        bait.transform.position = nextPos;
    }


    private void OnPlayerFishingThrow(Hashtable hashtable)
    {
        currentBaitTargetPos = (Vector3)hashtable["targetPos"];
        bait.transform.SetParent(null);

        baitStartPos = bait.transform.position;

        timeToTargetPos = Vector3.Distance(baitStartPos, currentBaitTargetPos) / baitSpeed;

        passedTime = 0;
        currentRodState = RodState.Throwing;
    }


    private void MoveToRod()
    {
        passedTime += Time.deltaTime;

        if (passedTime >= timeToTargetPos)
        {
            bait.transform.position = anchor.transform.position;
            currentRodState = RodState.Idle;
            EventManager.instance.InvokeEvent(EventEnums.onBaitReelFinished);
            return;
        }

        var t = (passedTime / timeToTargetPos);
        var nextPos = Vector3.Lerp(baitStartPos, anchor.transform.position, t);


        float dist = anchor.transform.position.x - baitStartPos.x;
        float arc = arcHeight * (nextPos.x - baitStartPos.x) * (nextPos.x - anchor.transform.position.x) / (-0.25f * dist * dist);

        nextPos = new Vector3(nextPos.x, nextPos.y + arc, nextPos.z);

        bait.transform.position = nextPos;


    }

    private void OnPlayerReel(Hashtable hashtable)
    {
        bait.transform.parent = this.transform;

        passedTime = 0;
        baitStartPos = bait.transform.position;

        timeToTargetPos = Vector3.Distance(baitStartPos, anchor.transform.position) / baitSpeed;

        currentRodState = RodState.Reeling;
    }


}
