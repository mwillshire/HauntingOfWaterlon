using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreMultiLevel : MonoBehaviour
{
    private string levelNum;
    private int highScore;

    private void Start()
    {
        string[] temp = SceneManager.GetActiveScene().name.Split('_');
        levelNum = temp[temp.Length - 1];

        if (PlayerPrefs.HasKey("HighScore_" + levelNum))
        {
            highScore = PlayerPrefs.GetInt("HighScore_" + levelNum);
        }
        else
        {
            highScore = 0;
            PlayerPrefs.SetInt("HighScore_" + levelNum, 1000);
        }
    }

    public void TryToSetNewHighScore(int newScore)
    {
        if (newScore <= highScore) return;

        PlayerPrefs.SetInt("HighScore_" + levelNum, newScore);
        Debug.Log("Set HighScore_" + levelNum + " to " + newScore);
    }
}
