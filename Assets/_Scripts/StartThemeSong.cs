
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartThemeSong : MonoBehaviour
{
    public static StartThemeSong themeInstance;

    [Header("Inscribed")]
    public string[] themeSongs;
    public int lastMenuBuildIndex;

    private int songPlayingIndex = 0;
    private AudioHandler aH;
    private void Awake()
    {
        if (themeInstance == null)
            themeInstance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        aH = FindFirstObjectByType<AudioHandler>();
        aH.Play(themeSongs[0]);
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().buildIndex <= lastMenuBuildIndex)
        {
            if (!IsThemeMusicPlaying())
            {
                aH.Play(themeSongs[0]);
            }

            return;
        }

        if (!aH.IsSoundPlaying(themeSongs[songPlayingIndex]))
        {
            int random = Random.Range(0, themeSongs.Length);
            aH.Play(themeSongs[random]);
            if (aH.IsSoundPlaying(themeSongs[random])) songPlayingIndex = random;
        }
    }

    private bool IsThemeMusicPlaying()
    {
        for (int i = 0;i < themeSongs.Length; i++)
        {
            if (aH.IsSoundPlaying(themeSongs[i])) return true;
        }

        return false;
    }
}
