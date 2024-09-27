using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LevelLockUnlock : MonoBehaviour
{
    [Header("Inscribed")]
    public TextMeshProUGUI tMP;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (FindScore() <= 1000)
        {
            button.enabled = false;
        }
        else
        {
            button.enabled = true;
        }
    }

    private int FindScore()
    {
        string[] temp = tMP.text.Split(" ");
        string scoreWithComma = temp[temp.Length - 1];

        string[] temp2 = scoreWithComma.Split(",");
        int score = Convert.ToInt32(temp2[temp2.Length - 1]);

        score += Convert.ToInt32(temp2[0]) * 1000;

        return score;
    }
}
