using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    
    void Start()
    {
        for(int i=0; i < Savesettings.players.Length; i++)
        {
            Savesettings.players[i] = "HUMAN";
        }
    }

    public void StarttheGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    
}
