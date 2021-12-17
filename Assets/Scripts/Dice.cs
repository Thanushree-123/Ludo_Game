using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;

    bool hasLanded;
    bool thrown;
    Vector3 initposition;
    public DiceScript[] diceSides;
    public int diceValue;

    void Start()
    {
        initposition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

   public void RollDice()
    {
        Reset();
        if(!thrown && !hasLanded)
        {
            thrown = true;
            rb.useGravity = true;
            rb.AddTorque(Random.Range(0,500),Random.Range(0,500),Random.Range(0,500));
        }
        else if(thrown && hasLanded)
        {
            Reset();
        }
    }

    void Reset()
    {
        transform.position = initposition;
        rb.isKinematic = false;
        thrown = false;
        hasLanded = false;
        rb.useGravity = false;
    }

    void Update()
    {
        if(rb.IsSleeping() && !hasLanded && thrown)
        {
            hasLanded = true;
            rb.useGravity = false;
            rb.isKinematic = true;

            SideValueCheck();
        }
        else if(rb.IsSleeping() && hasLanded && diceValue == 0)
        {
            RollAgain();
        }
    }

    void RollAgain()
    {
        Reset();
        thrown = true;
        rb.useGravity = true;
        rb.AddTorque(Random.Range(0,500),Random.Range(0,500),Random.Range(0,500));
    }

    void SideValueCheck()
    {
        diceValue = 0;
        foreach(DiceScript side in diceSides)
        {
            if(side.OnGround())
            {
                diceValue = side.sidevalue;
                GameManager.instance.RollDice(diceValue);
            }
        }
    }

    
}
