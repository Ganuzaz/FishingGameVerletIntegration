using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    public static StageManager instance;

    [Header("Cast Range")]
    public GameObject startRange;
    public GameObject endRange;

    [Header("Fish Boundary")]
    public GameObject topLeft;
    public GameObject topRight;
    public GameObject btmLeft;
    public GameObject btmRight;


    public GameObject fishParent;


    private List<FishController> fishList;

    private int totalToSpawn = 8;

    private FishDictionary fishDict;

    

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        fishDict = Resources.Load<DatabaseScriptableObject>("Database").fishDictionary;
    }

    private void Start()
    {
        fishList = new List<FishController>();
        for(int i = 0; i < totalToSpawn; i++)
        {
            SpawnFish();
        }


        EventManager.instance.AddListener(EventEnums.onBaitReachTarget,OnBaitReachedTarget);
    }

    /// <summary>
    /// Get throw target world position based on startRange and endRange object
    /// </summary>
    /// <param name="power">0-1</param>
    /// <returns></returns>
    public Vector3 GetThrowTargetPos(float power)
    {
        var delta = endRange.transform.position - startRange.transform.position;
        var currPower = Mathf.Clamp(power, 0,1);

        var targetPos = startRange.transform.position + delta * currPower;
        targetPos.y = startRange.transform.position.y;

        return targetPos;

    }

    public Vector3 GetRandomBoundaryPos()
    {
        var minX = topLeft.transform.position.x;
        var maxX = topRight.transform.position.x;
        var minZ = btmLeft.transform.position.z;
        var maxZ = topLeft.transform.position.z;

        return new Vector3(Random.Range(minX,maxX),topLeft.transform.position.y,Random.Range(minZ,maxZ));
    }

    public FishController SpawnFish()
    {

        var index = Random.Range(0, fishDict.Count);

        var spawnedFish = Instantiate(fishDict[(FishType)index]);

        spawnedFish.transform.position = GetRandomBoundaryPos();

        spawnedFish.transform.parent = fishParent.transform;

        var fishController = spawnedFish.GetComponent<FishController>();
        fishList.Add(fishController);

        return fishController;

    }

    void OnBaitReachedTarget(Hashtable hashtable)
    {
        var bait = (GameObject)hashtable["bait"];

        var randomFish = fishList[Random.Range(0, fishList.Count)];

        randomFish.StartSwimToBait(bait);
    }

}
