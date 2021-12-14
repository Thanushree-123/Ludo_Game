using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [System.Serializable]
    public class Player
    {
        public string playerName;
        public Stone[] myStones;
        public bool hasturn;
        public enum PlayerTypes
        {
            Human,
            CPU
        }
        public PlayerTypes playertype;
        public bool hasWon;
    }

    public List<Player> playerList = new List<Player>();

    //STATEMACHINE 
    public enum States
    {
        Waiting,
        Roll_Dice,
        Switch_Player
    }
    public States state;

    public int activePlayer;
    bool switchingPlayer;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        switch(state)
        {
            case States.Roll_Dice:
            break;
            case States.Waiting:
            break;
            case States.Switch_Player:
            break;
        }
    }
}
