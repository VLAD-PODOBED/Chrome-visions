using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LapCounter : MonoBehaviour
{
    public UIDocument uiDocument;
    public Label lapLabel;
    public int lapCount = 0;
    public const int totalLaps = 3;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        lapLabel = root.Q<Label>("LapCount");
        UpdateLapLabel();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("arch"))
        {
            lapCount++;
            UpdateLapLabel();

            if (lapCount == totalLaps)
            {
                LoadVictoryScene();
            }
        }
    }

    void UpdateLapLabel()
    {
        lapLabel.text = $"{lapCount}/{totalLaps}";
    }

    void LoadVictoryScene()
    {
        SceneManager.LoadScene("EndScene");
    }
}