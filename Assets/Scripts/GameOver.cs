using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class GameOver : MonoBehaviour
{
    public Text first, second, third;
    void Start()
    {
        first.text = "1st : " + Savesettings.winners[0];
        second.text = "2nd : " + Savesettings.winners[1];
        third.text = "3rd : " + Savesettings.winners[2];
    }

    public void BackButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    
}
