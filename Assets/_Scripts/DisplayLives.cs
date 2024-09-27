using UnityEngine;
using UnityEngine.UI;

public class DisplayLives : MonoBehaviour
{
    private Text display;
    private Player player;

    private void Start()
    {
        display = GetComponent<Text>();
        player = FindObjectOfType<Player>();
    }

    private void FixedUpdate()
    {
        display.text = "Lives: " + player.lives;
    }
}
