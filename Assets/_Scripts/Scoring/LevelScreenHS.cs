using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;


public class LevelScreenHS : MonoBehaviour
{
    [Header("Inscribed")]
    public string levelNum;

    private TextMeshProUGUI highScoreText;

    private void Start()
    {
        highScoreText = GetComponent<TextMeshProUGUI>();

        if (PlayerPrefs.HasKey("HighScore_" + levelNum))
            highScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore_" + levelNum).ToString("#,0");
        else
            highScoreText.text = "HighScore: 0";
    }

    private void FixedUpdate()
    {
        if (PlayerPrefs.HasKey("HighScore_" + levelNum))
            highScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore_" + levelNum).ToString("#,0");
        else
            highScoreText.text = "HighScore: 0";
    }
}
