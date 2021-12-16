using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    bool onground;
    public int sidevalue;

    void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("DiceGround"))
        {
            onground = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("DiceGround"))
        {
            onground = false;
        }
    }

    public bool OnGround()
    {
        return onground;
    }

}
