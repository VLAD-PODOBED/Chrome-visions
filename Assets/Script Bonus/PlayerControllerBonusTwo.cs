using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class PlayerControllerBonusTwo : MonoBehaviour
{
    public float speed = 10.0f;
    public float maxSpeed = 5.0f;
    private Rigidbody rb;
    private bool canActivateBonus = false;
    private bool bonusActive = false;
    public AudioClip passThroughSound;
    private float bonusEndTime;

    public UIDocument uiDocument;
    private VisualElement bonus1Image;
    private VisualElement bonus3Image;

    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        var root = uiDocument.rootVisualElement;
        bonus1Image = root.Q<VisualElement>("Bonus1");
        bonus3Image = root.Q<VisualElement>("Bonus3");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BonusTwo"))
        {
            Debug.Log("BonusTwo collided with player!");
            canActivateBonus = true;
            collision.gameObject.GetComponent<BonusTwoRespawn>().StartRespawn();
        }
    }

    void Update()
    {
        // Если один из UI-элементов бонусов видим – не активируем бонус
        if (IsImageVisible(bonus1Image) || IsImageVisible(bonus3Image))
        {
            Debug.Log("Cannot activate bonus because Bonus1 or Bonus3 image is visible.");
            return;
        }

        if (canActivateBonus && Input.GetKeyDown(KeyCode.E) && !bonusActive)
        {
            if (passThroughSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(passThroughSound);
            }
            bonusActive = true;
            bonusEndTime = Time.time + 5f;
            Debug.Log("Bonus activated");

            // Автоматически выбираем случайного бота
            GameObject selectedBot = SelectRandomBot();
            if (selectedBot != null)
            {
                Rigidbody selectedBotRb = selectedBot.GetComponent<Rigidbody>();
                if (selectedBotRb != null)
                {
                    Vector3 originalPosition = selectedBotRb.transform.position;
                    Vector3 halfSubmergePosition = originalPosition - Vector3.up * 0.5f;
                    StartCoroutine(FreezeAndReturnBall(selectedBotRb, halfSubmergePosition, 5f));
                }
                else
                {
                    Debug.Log("Selected bot does not have a Rigidbody component.");
                }
            }
            else
            {
                Debug.Log("No bot found for bonus effect.");
            }

            canActivateBonus = false;
            bonusActive = false;
        }
    }

    bool IsImageVisible(VisualElement imageElement)
    {
        return imageElement != null && imageElement.resolvedStyle.display != DisplayStyle.None;
    }

    // Метод для выбора случайного бота с тегами "BOT1", "BOT2" и "BOT3"
    private GameObject SelectRandomBot()
    {
        List<GameObject> bots = new List<GameObject>();
        bots.AddRange(GameObject.FindGameObjectsWithTag("BOT1"));
        bots.AddRange(GameObject.FindGameObjectsWithTag("BOT2"));
        bots.AddRange(GameObject.FindGameObjectsWithTag("BOT3"));

        if (bots.Count == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, bots.Count);
        return bots[randomIndex];
    }

    IEnumerator FreezeAndReturnBall(Rigidbody ballRb, Vector3 submergePosition, float freezeDuration)
    {
        Vector3 originalPosition = ballRb.transform.position;
        bool originalIsKinematic = ballRb.isKinematic;

        ballRb.velocity = Vector3.zero;
        ballRb.isKinematic = true;
        ballRb.transform.position = submergePosition;

        Debug.Log("Bot submerged");

        float elapsedTime = 0f;
        while (elapsedTime < freezeDuration)
        {
            ballRb.transform.position = submergePosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ballRb.isKinematic = originalIsKinematic;
        ballRb.transform.position = originalPosition;

        Debug.Log("Bot returned to original position");
    }
}
