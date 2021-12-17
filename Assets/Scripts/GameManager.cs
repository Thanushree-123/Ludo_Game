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
    bool turnPossible = true;

    //GameObject for button
    public GameObject rollbutton;
    [HideInInspector]public int rollHumandice;

    public Dice dice;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ActivateButton(false);

        int randomPlayer = Random.Range(0, playerList.Count);
        activePlayer = randomPlayer;
        Instruction.instance.showMessage(playerList[activePlayer].playerName + " starts first!");
    }

    void Update()
    {
        if(playerList[activePlayer].playertype == Player.PlayerTypes.CPU)
        {
        switch(state)
        {
            case States.Roll_Dice:
            {
                if(turnPossible)
                {
                    StartCoroutine(RollDiceDelay());
                    state = States.Waiting;
                }
                
            }
            break;
            case States.Waiting:
            {

            }
            break;
            case States.Switch_Player:
            {
                if(turnPossible)
                {
                    StartCoroutine(SwitchPlayer());
                    state = States.Waiting;
                }
                
            }
            break;
        }
        }

        if(playerList[activePlayer].playertype == Player.PlayerTypes.Human)
        {
        switch(state)
        {
            case States.Roll_Dice:
            {
                if(turnPossible)
                {
                    ActivateButton(true);
                    state = States.Waiting;
                }
                
            }
            break;
            case States.Waiting:
            {

            }
            break;
            case States.Switch_Player:
            {
                if(turnPossible)
                {
                    StartCoroutine(SwitchPlayer());
                    state = States.Waiting;
                }
                
            }
            break;
        }
        }
    }

    void CPUDice()
    {
        dice.RollDice();
    }

  public void RollDice(int _diceNumber)
    {
        int dicenumber = _diceNumber;
        //int dicenumber = 6;
        if(playerList[activePlayer].playertype == Player.PlayerTypes.CPU)
        {
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
        }
        if(playerList[activePlayer].playertype == Player.PlayerTypes.Human)
        {
            rollHumandice = _diceNumber;
            HumanRollDice();
        }
        
        Debug.Log("dice rolled number" + dicenumber);
        Instruction.instance.showMessage(playerList[activePlayer].playerName + " has rolled " + _diceNumber);
    }

    IEnumerator RollDiceDelay()
    {
        yield return new WaitForSeconds(1);
        //RollDice();
        CPUDice();
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
        Instruction.instance.showMessage(playerList[activePlayer].playerName + " turn!");
        state = States.Roll_Dice;

    }

    public void ReportTurnPossible(bool possible)
    {
        turnPossible = possible;
    }

    public void ReportWinning()
    {
        playerList[activePlayer].hasWon = true;
    }

    //-------------HUMAN INPUT------------------
    void ActivateButton(bool on)
    {
        rollbutton.SetActive(on);
    }

    public void DeactivateAllselector()
    {
        for(int i=0; i< playerList.Count; i++)
        {
            for(int j=0; j< playerList[i].myStones.Length; j++)
            {
                playerList[i].myStones[j].SetSelector(false);
            }
        }
    }

    //This sits on the roll dice button
    public void Humanroll()
    {
        dice.RollDice();
        ActivateButton(false);
    }

    public void HumanRollDice()
    {
        
        //roll dice
        //rollHumandice = Random.Range(1,7);
        //rollHumandice = 6;
        List <Stone> movablestones = new List<Stone>();
        
        //check anyone in the start node
        bool startnodefull = false;
        for(int i=0; i< playerList[activePlayer].myStones.Length; i++)
        {
            if(playerList[activePlayer].myStones[i].currentNode == playerList[activePlayer].myStones[i].startNode)
            {
                startnodefull = true;
                break;
            }
        }

        //number<6
        if(rollHumandice < 6)
        {
        
            movablestones.AddRange(Possiblestone());

        }
       

        //dicenumber ==6 && no one at the startnode
        if(rollHumandice == 6 && !startnodefull)
        {
            //inside base check
            for(int i=0; i< playerList[activePlayer].myStones.Length; i++)
            {
                if(!playerList[activePlayer].myStones[i].ReturnIsout())
                {
                    movablestones.Add(playerList[activePlayer].myStones[i]);
                }
            }

            //outside check
            movablestones.AddRange(Possiblestone());
        }
        //dicenumber == 6 and startnode
        else if(rollHumandice == 6 && startnodefull)
        {
            movablestones.AddRange(Possiblestone());
        }

        //Activate all possible selectors
        if(movablestones.Count > 0)
        {
            for(int i=0; i< movablestones.Count; i++)
            {
                movablestones[i].SetSelector(true);
            }
        }
        else{
            state = States.Switch_Player;
        }

    }

    List <Stone> Possiblestone()
    {
        List<Stone> templist = new List<Stone>();

        for(int i=0; i< playerList[activePlayer].myStones.Length; i++)
        {
            if(playerList[activePlayer].myStones[i].ReturnIsout())
            {
                if(playerList[activePlayer].myStones[i].CheckPossibleKick(playerList[activePlayer].myStones[i].stoneid,rollHumandice))
                {
                    templist.Add(playerList[activePlayer].myStones[i]);
                    continue;
                }
                if(playerList[activePlayer].myStones[i].CheckPossibleMove(rollHumandice))
                {
                   templist.Add(playerList[activePlayer].myStones[i]);
                    
                }

            }
        }

        return templist;
    }
}
