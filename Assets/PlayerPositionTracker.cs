using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPositionUI : MonoBehaviour
{
    public GameObject playerCar;
    public List<GameObject> allCars;
    public UIDocument uiDocument;
    private Label lapPointLabel;

    void Start()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UI Document is not assigned!");
            return;
        }

        var root = uiDocument.rootVisualElement;
        lapPointLabel = root.Q<Label>("LapCount1");

        if (lapPointLabel == null)
        {
            Debug.LogError("LapPointLabel not found in UI Document!");
        }
    }

    void Update()
    {
        if (lapPointLabel == null)
        {
            Debug.LogError("LapPointLabel is not initialized!");
            return;
        }

        int playerPosition = GetPlayerPosition();
        lapPointLabel.text = "Position: " + playerPosition;
    }

    int GetPlayerPosition()
    {
        List<float> distances = new List<float>();

        foreach (GameObject car in allCars)
        {
            CarController carController = car.GetComponent<CarController>();
            if (carController != null)
            {
                distances.Add(carController.GetDistanceTravelled());
            }
            else
            {
                Debug.LogError("CarController component missing on one of the cars!");
            }
        }
        CarController playerController = playerCar.GetComponent<CarController>();
        if (playerController == null)
        {
            Debug.LogError("Player car does not have CarController component!");
            return -1;
        }

        float playerDistance = playerController.GetDistanceTravelled();
        distances.Sort((a, b) => b.CompareTo(a));
        int position = distances.IndexOf(playerDistance) + 1;
        return position;
    }
}