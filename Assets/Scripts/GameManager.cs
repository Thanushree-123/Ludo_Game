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
        if(playerList[activePlayer].playertype == Player.PlayerTypes.CPU)
        {
        switch(state)
        {
            case States.Roll_Dice:
            {
                StartCoroutine(RollDiceDelay());
                state = States.Waiting;
            }
            break;
            case States.Waiting:
            {

            }
            break;
            case States.Switch_Player:
            {
                StartCoroutine(SwitchPlayer());
                state = States.Waiting;
            }
            break;
        }
        }
    }

    void RollDice()
    {
        int dicenumber = Random.Range(1,7);
        //int dicenumber = 6;

        if(dicenumber == 6)
        {
            //check the start node
            CheckStartNode(dicenumber);
        }

        if(dicenumber < 6)
        {
            //check for kick
            MoveaStone(dicenumber);
        }
        Debug.Log("dice rolled number" + dicenumber);
    }

    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(2);
        RollDice();
    }

    void CheckStartNode(int dicenumber)
    {
        bool startnodefull = false;
        for(int i=0; i< playerList[activePlayer].myStones.Length; i++)
        {
            if(playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startnodefull = true;
                break;
            }
        }
        if(startnodefull)
        {
            MoveaStone(dicenumber);
        }
        else
        {
            for( int i=0; i< playerList[activePlayer].myStones.Length; i++)
            {
                if(!playerList[activePlayer].myStones[i].ReturnIsout())
                {
                    playerList[activePlayer].myStones[i].LeaveBase();
                    state = States.Waiting;
                    return;
                }
            }
            MoveaStone(dicenumber);
        }
    }

    void MoveaStone(int dicenumber)
    {
        List <Stone> movablestones = new List<Stone>();
        List <Stone> moveKickstones = new List<Stone>();

        for(int i=0; i< playerList[activePlayer].myStones.Length; i++)
        {
            if(playerList[activePlayer].myStones[i].ReturnIsout())
            {
                if(playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneid,dicenumber))
                {
                    moveKickstones.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }
                if(playerList[activePlayer].myStones[i].CheckPossibleMove(dicenumber))
                {
                    movablestones.Add(playerList[activePlayer].myStones[i]);
                    
                }

            }
        }

        if(moveKickstones.Count>0)
        {
            int num = Random.Range(0, moveKickstones.Count);
            moveKickstones[num].StartTheMove(dicenumber);
            state = States.Waiting;
            return;
        }

        if(movablestones.Count>0)
        {
            int num = Random.Range(0, movablestones.Count);
            movablestones[num].StartTheMove(dicenumber);
            state = States.Waiting;
            return;
        }
        //switching the player
        state = States.Switch_Player;
    }

    IEnumerator SwitchPlayer()
    {
        if(switchingPlayer)
        {
            yield break;
        }

        switchingPlayer = true;
        yield return new WaitForSeconds(2);
        SetNextActivePlayer();
        switchingPlayer = false;
    }

    void SetNextActivePlayer()
    {
        activePlayer++;
        activePlayer %= playerList.Count;

        int available = 0;
        for (int i=0; i< playerList.Count;i++)
        {
            if(!playerList[i].hasWon)
            {
                available++;
            }
        }

        if(playerList[activePlayer].hasWon && available >1)
        {
            SetNextActivePlayer();
            return;
        }
        else if(available<2)
        {
            //game over screen
            state = States.Waiting;
            return;
        }

        state = States.Roll_Dice;

    }
}
