using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Database", menuName = "ScritableObjects/DatabaseScriptableObject")]
public class DatabaseScriptableObject : ScriptableObject
{
    public FishDictionary fishDictionary;
}

[System.Serializable]
public class FishDictionary : SerializableDictionaryBase<FishType, GameObject> { }
