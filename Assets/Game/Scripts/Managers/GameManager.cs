using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Paused,
    Playing,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState gameState;


    private void Awake()
    {
        instance = this;
        gameState = GameState.Playing;
    }

    private void Start()
    {
        EventManager.instance.AddListener(EventEnums.onGameFinished,SetToPause );
        EventManager.instance.AddListener(EventEnums.onGameReset, SetToPlaying);
    }

    private void SetToPause(Hashtable hashtable)
    {
        gameState = GameState.Paused;
    }

    private void SetToPlaying(Hashtable hashtable)
    {
        gameState = GameState.Playing;
    }

    public bool CanReceiveInput()
    {
        return gameState != GameState.Paused;
    }

}
