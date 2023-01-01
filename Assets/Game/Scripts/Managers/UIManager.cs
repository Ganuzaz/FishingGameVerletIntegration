using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Slider chargeSlider;

    [Header("Fish Canvas")]
    public GameObject caughtFishCanvas;
    public TextMeshProUGUI descText;
    public FishDictionary fishDict;


    private bool charge = false;

    private float direction = 1;

    private float passedTime = 0;

    private bool isFishReeled = false;
    private FishType caughtFishType;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventManager.instance.AddListener(EventEnums.onPlayerFishingStartCast,StartSlider);
        EventManager.instance.AddListener(EventEnums.onPlayerChargeRelease, (Hashtable hashtable) => { charge = false; });
        EventManager.instance.AddListener(EventEnums.onBaitReachTarget, DisableSlider);
        EventManager.instance.AddListener(EventEnums.onFishGotReeled, OnFishGotReeled);
        EventManager.instance.AddListener(EventEnums.onGameReset, OnGameReset);

        EventManager.instance.AddListener(EventEnums.onBaitReelFinished, ShowFinishScreen);

        DisableSlider();
    }

    private void OnGameReset(Hashtable hashtable)
    {
        isFishReeled = false;
    }


    private void Update()
    {
        FillBar();    
    }

    private void FillBar()
    {
        if (!charge)
            return;

        passedTime += Time.deltaTime * direction;

        var totalValue = 1 * Mathf.Pow(passedTime, 2);

        if (totalValue <= 0.001f && direction == -1)
            totalValue = 0;

        chargeSlider.value = totalValue;        


        if (totalValue >= 1)
            direction = -1;
        else if (totalValue <= 0)
        {
            direction = 1;
            passedTime = 0;
        }

    }


    private void StartSlider(Hashtable hashtable)
    {
        chargeSlider.gameObject.SetActive(true);
        chargeSlider.value = 0;
        direction = 1;
        charge = true;

        passedTime = 0;
    }

    private void DisableSlider(Hashtable hashtable = null)
    {
        charge = false;
        chargeSlider.gameObject.SetActive(false);
    }

    private void OnFishGotReeled(Hashtable hashtable = null)
    {
        caughtFishType = (FishType)hashtable["fishType"];

        isFishReeled = true;
    }


    private void ShowFinishScreen(Hashtable hashtable)
    {
        caughtFishCanvas.SetActive(true);

        foreach(var fish in fishDict)
        {            
            fish.Value.SetActive(isFishReeled & fish.Key == caughtFishType);
        }

        descText.text = isFishReeled ? "You Caught a Fish!" : "No Fish Caught..";

        EventManager.instance.InvokeEvent(EventEnums.onGameFinished);
    }
    

    public void OnCloseButtonClicked()
    {
        EventManager.instance.InvokeEvent(EventEnums.onGameReset);
    }

}
