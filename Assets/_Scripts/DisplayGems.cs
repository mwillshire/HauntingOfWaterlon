using UnityEngine;
using UnityEngine.UI;

public class DisplayGems : MonoBehaviour
{
    private Text display;
    private EventHandler eH;

    private void Start()
    {
        display = GetComponent<Text>();
        eH = FindObjectOfType<EventHandler>();
    }

    private void FixedUpdate()
    {
        display.text = "Gems: " + eH.gemsCollected;
    }
}
