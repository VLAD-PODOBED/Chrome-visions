using UnityEngine;
using UnityEngine.UIElements;

public class GameTimerUI : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label timerLabel;
    private Label lapCountLabel; 
    private float startTime;
    private bool isTimerRunning = true; 

    void Start()
    {
        startTime = Time.time;
        var root = uiDocument.rootVisualElement;
        timerLabel = root.Q<Label>("CurrentLapTime");
        lapCountLabel = root.Q<Label>("LapCount"); 
    }

    void Update()
    {
        if (isTimerRunning)
        {
            float t = Time.time - startTime;
            int minutes = ((int)t / 60);
            int seconds = ((int)t % 60);
            int milliseconds = (int)((t - Mathf.Floor(t)) * 1000);
            timerLabel.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

            if (lapCountLabel.text == "3/3")
            {
                isTimerRunning = false;
                Debug.Log("Timer stopped as LapCount is 3/3");
            }
        }
    }
}