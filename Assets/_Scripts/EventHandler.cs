using UnityEngine;
using UnityEngine.SceneManagement;

public class EventHandler : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject[] pausedObjects;

    [Header("Dynamic")]
    [Range(0f, 2f)]
    public int gemsCollected = 0;
    public int startingScore = 2000; //This value will subtracted from by the amount of time passed
    public float timeElapsed = 0f;

    private int score = 0;

    public static bool isPaused = false;

    private void Awake()
    {
        UnPause();
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            isPaused = true;

            FindObjectOfType<AudioHandler>().Play("InterfaceClick");

            foreach (GameObject obj in pausedObjects)
            {
                obj.SetActive(true);
            }

            Pause();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            isPaused = false;

            FindObjectOfType<AudioHandler>().Play("InterfaceBack");

            foreach (GameObject obj in pausedObjects)
            {
                obj.SetActive(false);
            }

            UnPause();
        }
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;
    }

    public void CalculateScore()
    {
        int tElapsed = (int)Mathf.Round(timeElapsed) * 1000;
        score = gemsCollected * 50000 + (startingScore - tElapsed);

        PlayerPrefs.SetInt("PlayerScore", score);
        FindFirstObjectByType<HighScoreMultiLevel>().TryToSetNewHighScore(score);

        string[] temp = SceneManager.GetActiveScene().name.Split('_');
        StaticVariables.prevSceneLevel = temp[temp.Length - 1];
        StaticVariables.prevSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        GetComponent<SceneManagement>().LoadScene(5);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }
    public void UnPause()
    {
        Time.timeScale = 1f;
    }
}
