using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ResetHighScore : MonoBehaviour
{
    [Header("If True will reset highscores on start")]
    public bool resetHighScores = false;
    public string[] highScoresToReset;

    private void Awake()
    {
        if (resetHighScores)
        {
            for (int i = 0; i < highScoresToReset.Length; i++)
            {
                PlayerPrefs.SetInt("HighScore_" + highScoresToReset[i], 0);
            }
        }
    }
}
