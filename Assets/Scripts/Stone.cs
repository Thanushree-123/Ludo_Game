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
    bool hasturn;

    [Header("SELECTOR")]
    public GameObject selector;


    void Start()
    {
        startNodeIndex = commonRoute.RequestPosition(startNode.gameObject.transform);
        CreateFullRoute();
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

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            steps = Random.Range(1,7);//range function take one value less
            Debug.Log("dice number=" + steps);
            if(doneSteps + steps < fullRoute.Count)
            {
                StartCoroutine(Move());
            }
            else
            {
                Debug.Log("number is to high");
            }
        }
    }

    IEnumerator Move()
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
            //kick the other stone
        }

        currentNode.stone = null;
        currentNode.isTaken = false;

        goalnode.stone = this;
        goalnode.isTaken = true;

        currentNode = goalnode;
        goalnode = null;

        GameManager.instance.state = GameManager.States.Switch_Player;
        isMoving = false;
    }

    bool MoveToNextNode(Vector3 goalpos, float speed)
    {
        return goalpos != (transform.position = Vector3.MoveTowards(transform.position,goalpos,speed * Time.deltaTime));
        
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
        StartCoroutine(Move());
    }


}
