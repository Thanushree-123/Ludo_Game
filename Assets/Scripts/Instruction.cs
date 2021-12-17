using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Instruction : MonoBehaviour
{
    public static Instruction instance;
    public Text infotext;

    void Awake()
    {
        instance = this;
        infotext.text = "";
    }

    public void showMessage(string _text)
    {
        infotext.text = _text;
    }
}
