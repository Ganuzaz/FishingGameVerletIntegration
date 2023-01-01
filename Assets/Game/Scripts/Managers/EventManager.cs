using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum EventEnums
{
    onPlayerFishingStartCast, //1. Casting animation & Waiting for left mouse button to be released
    onPlayerChargeRelease, // 2. Left mouse button is released 
    onPlayerFishingBaitThrow, //3. waiting for bait to land on water
    onBaitReachTarget, //4. bait landed on water
    onFishAttached, //5. On Fish attached on the bait
    onPlayerFishingBaitReel, //5. Bait reel
    onBaitReelFinished, //6. Bait reached the fishing pole back

    onFishGotReeled,//6. when fish reach the fishing pole

    onGameFinished,//7. on game finished and ready to be reset to the initial state

    onGameReset// resets the game to be played again
}

[System.Serializable]
public class CustomEvent : UnityEvent<Hashtable> { }

public class EventManager : MonoBehaviour
{
    private Dictionary<EventEnums, CustomEvent> eventDictionary;

    public static EventManager instance;

    private Dictionary<EventEnums, List<UnityAction<Hashtable>>> onceEventDictionary;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this);

        InitEvents();
    }

    private void InitEvents()
    {
        eventDictionary = new Dictionary<EventEnums, CustomEvent>();
        onceEventDictionary = new Dictionary<EventEnums, List<UnityAction<Hashtable>>>();
        foreach(EventEnums eventEnum in System.Enum.GetValues(typeof(EventEnums)))
        {
            eventDictionary.Add(eventEnum, new CustomEvent());
            onceEventDictionary.Add(eventEnum, new List<UnityAction<Hashtable>>());
        }
    }

    public void AddListener(EventEnums eventName, UnityAction<Hashtable> listener)
    {
        CustomEvent currentEvent;
        eventDictionary.TryGetValue(eventName, out currentEvent);

        if (currentEvent == null)
            return;

        currentEvent.AddListener(listener);
    }

    public void RemoveListener(EventEnums eventName, UnityAction<Hashtable> listener)
    {
        CustomEvent currentEvent;
        eventDictionary.TryGetValue(eventName, out currentEvent);

        if (currentEvent == null)
            return;

        currentEvent.RemoveListener(listener);
    }

    public void InvokeEvent(EventEnums eventName, Hashtable hashtable = null)
    {
        CustomEvent currentEvent;
        eventDictionary.TryGetValue(eventName, out currentEvent);

        if (currentEvent == null)
            return;

        currentEvent.Invoke(hashtable);

        var onceEventList = onceEventDictionary[eventName];

        for(int i=0;i< onceEventList.Count; i++)
        {
            currentEvent.RemoveListener(onceEventList[i]);
        }

        onceEventList.Clear();     
    }

    public void AddListenerOnce(EventEnums eventName, UnityAction<Hashtable> listener)
    {
        CustomEvent currentEvent;
        eventDictionary.TryGetValue(eventName, out currentEvent);

        if (currentEvent == null)
            return;
        

        currentEvent.AddListener(listener);
        onceEventDictionary[eventName].Add(listener);

    }

}

