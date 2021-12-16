using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int stoneid;
    [Header("ROUTES")]
    public OuterRoute commonRoute;
    public OuterRoute finalRoute;

    public List<Node> fullRoute = new List<Node>();
    [Header("NODES")]
    public Node startNode;
    public Node basenode;
    public Node currentNode;
    public Node goalnode;

    int routePosition;
    int startNodeIndex;

    int steps;
    int doneSteps;
    
    [Header("BOOLS")]
    public bool isout;
    bool isMoving;
    bool hasturn; //for human input

    [Header("SELECTOR")]
    public GameObject selector;

    //Hopping movement
    float amplitude = 0.5f;
    float ctime = 0f;


    void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();

        SetSelector(false);
    }

    void CreateFullRoute()
    {
        for (int i=0; i< commonRoute.childNodesList.Count; i++)
        {
            int tempPos = startNodeIndex + i;
            tempPos %= commonRoute.childNodesList.Count;

            fullRoute.Add(commonRoute.childNodesList[tempPos].GetComponent<Node>());
        }

        for (int i=0; i< finalRoute.childNodesList.Count; i++)
        {
            
            fullRoute.Add(finalRoute.childNodesList[i].GetComponent<Node>());
        }
    }


    IEnumerator Move(int dicenumber)
    {
        if(isMoving)
        {
            yield break;
        }

        isMoving = true;

        while(steps>0)
        {
            routePosition++;
            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            Vector3 startpos = fullRoute[routePosition - 1].gameObject.transform.position;
            //while(MoveToNextNode(nextPos,8f))
            //{
              //  yield return null;
            //}

            while(hoppingtonextnode(startpos,nextPos, 8f)){yield return null;}
            yield return new WaitForSeconds(0.1f);
            ctime =0;
            steps--;
            doneSteps++;
        }

        goalnode = fullRoute[routePosition];
        if(goalnode.isTaken)
        {
            //kick the other stone
            goalnode.stone.ReturnToBase();
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalnode.stone = this;
        goalnode.isTaken = true;

        currentNode = goalnode;
        goalnode = null;

        //winning condition check
        if(WinningCondition())
        {
            GameManager.instance.ReportWinning();
        }

        //switch the player
        if(dicenumber < 6)
        {
            GameManager.instance.state = GameManager.States.Switch_Player;
        }
        else{
            GameManager.instance.state = GameManager.States.Roll_Dice;
        }
        
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalpos, float speed)
    {
        return goalpos != (transform.position = Vector3.MoveTowards(transform.position,goalpos,speed * Time.deltaTime));
        
    }

    bool hoppingtonextnode(Vector3 startpos, Vector3 goalpos, float speed)
    {
        ctime += speed * Time.deltaTime;
        Vector3 myposition = Vector3.Lerp(startpos, goalpos, ctime);
        myposition.y += amplitude * Mathf.Sin(Mathf.Clamp01(ctime)* Mathf.PI);
        return goalpos != (transform.position = Vector3.Lerp(transform.position, myposition, ctime) );
    }

    public bool ReturnIsout()
    {
        return isout;
    }

    public void LeaveBase()
    {
        steps =1;
        isout = true;
        routePosition = 0;

        StartCoroutine(MoveOut());
    }

    IEnumerator MoveOut()
    {
        if(isMoving)
        {
            yield break;
        }

        isMoving = true;

        while(steps>0)
        {
            //routePosition++;
            Vector3 nextPos = fullRoute[routePosition].gameObject.transform.position;
            while(MoveToNextNode(nextPos,8f))
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            steps--;
            doneSteps++;
        }

        goalnode = fullRoute[routePosition];
        if(goalnode.isTaken)
        {
            //return to start basenode
            goalnode.stone.ReturnToBase();
        }

        goalnode.stone = this;
        goalnode.isTaken = true;

        currentNode = goalnode;
        goalnode = null;
 
        GameManager.instance.state = GameManager.States.Roll_Dice;
        isMoving = false;
    }

    public bool CheckPossibleMove(int dicenumber)
    {
        int tempos = routePosition + dicenumber;
        if(tempos >= fullRoute.Count)
        {
            return false;
        }
        return !fullRoute[tempos].isTaken;
    }

    public bool CheckPossibleKick(int stoneID, int dicenumber)
    {
        int tempos = routePosition + dicenumber;
        if(tempos >= fullRoute.Count)
        {
            return false;
        }
        if(fullRoute[tempos].isTaken)
        {
            if(stoneID == fullRoute[tempos].stone.stoneid)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public void StartTheMove(int dicenumber)
    {
        steps = dicenumber;
        StartCoroutine(Move(dicenumber));
    }

    public void ReturnToBase()
    {
        StartCoroutine(Return());
    }

    IEnumerator Return()
    {
        GameManager.instance.ReportTurnPossible(false);
        routePosition = 0;
        currentNode = null;
        goalnode = null;
        isout = false;
        doneSteps = 0;

        Vector3 basenodepos = basenode.gameObject.transform.position;
        while(MoveToNextNode(basenodepos,100f))
        {
            yield return null;
        }
        GameManager.instance.ReportTurnPossible(true);
    }

    bool WinningCondition()
    {
        for(int i=0; i< finalRoute.childNodesList.Count; i++)
        {
            if(!finalRoute.childNodesList[i].GetComponent<Node>().isTaken)
            {
                return false;
            }
        }
        return true;
    }

    //-------------------HUMAN INPUT-------------------

    public void SetSelector(bool on)
    {
        selector.SetActive(on);

        hasturn = on;
    }

    void OnMouseDown()
    {
        if(hasturn)
        {
            if(!isout)
            {
                LeaveBase();
            }
            else
            {
                StartTheMove(GameManager.instance.rollHumandice);
            }
            GameManager.instance.DeactivateAllselector();
        }
        
    }


}
