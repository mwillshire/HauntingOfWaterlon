using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayAgainButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SceneManagement>().sceneID = StaticVariables.prevSceneBuildIndex;
    }
}
