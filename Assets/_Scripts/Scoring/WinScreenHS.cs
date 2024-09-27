using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class WinScreenHS : MonoBehaviour
{
    private string levelNum;
    private TextMeshProUGUI highScoreText;

    private void Start()
    {
        highScoreText = GetComponent<TextMeshProUGUI>();
        levelNum = StaticVariables.prevSceneLevel;

        if (PlayerPrefs.HasKey("HighScore_" + levelNum))
            highScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore_" + levelNum).ToString("#,0");
        else
            highScoreText.text = "HighScore: 0";
    }
}
