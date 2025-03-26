using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class PlayerControllerBonusThird : MonoBehaviour
{
    private bool canSelectBot = false;
    private bool bonusActive = false;
    private float bonusEndTime;

    public AudioClip passThroughSound;
    [SerializeField] private AudioSource audioSource;

    public UIDocument uiDocument;
    private VisualElement potImage;
    private VisualElement topImage;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        potImage = root.Q<VisualElement>("Bonus1");
        topImage = root.Q<VisualElement>("Bonus2");

        if (potImage == null)
        {
            Debug.LogError("Pot image not found in UIDocument!");
        }
        if (topImage == null)
        {
            Debug.LogError("Top image not found in UIDocument!");
        }
    }

    void Update()
    {
        // Если видны какие-либо изображения бонусов, не активируем бонус
        if (IsImageVisible(potImage) || IsImageVisible(topImage))
        {
            Debug.Log("Нельзя активировать бонус, так как одно из изображений видно.");
            return;
        }

        // При нажатии Е, если можно выбрать бота и бонус не активен
        if (canSelectBot && Input.GetKeyDown(KeyCode.E) && !bonusActive)
        {
            audioSource.PlayOneShot(passThroughSound);
            bonusActive = true;

            // Выбираем случайного бота для применения бонуса
            GameObject selectedBot = SelectRandomBot();
            if (selectedBot != null)
            {
                StartCoroutine(SwapPositions(selectedBot.transform, transform, 0.5f));
                Debug.Log("Бонус активирован: позиции обменялись с ботом " + selectedBot.name);
            }
            else
            {
                Debug.Log("Не найден подходящий бот для применения бонуса.");
            }
            canSelectBot = false;
            bonusActive = false;
        }
    }

    bool IsImageVisible(VisualElement imageElement)
    {
        return imageElement != null && imageElement.resolvedStyle.display != DisplayStyle.None;
    }

    // Метод для выбора случайного бота. Здесь можно добавить дополнительную фильтрацию (например, по количеству кругов)
    private GameObject SelectRandomBot()
    {
        List<GameObject> bots = new List<GameObject>();
        bots.AddRange(GameObject.FindGameObjectsWithTag("BOT1"));
        bots.AddRange(GameObject.FindGameObjectsWithTag("BOT2"));
        bots.AddRange(GameObject.FindGameObjectsWithTag("BOT3"));

        // Если будет доступна информация о количестве кругов, можно оставить только ботов с кругом >= круг игрока

        if (bots.Count == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, bots.Count);
        return bots[randomIndex];
    }

    IEnumerator SwapPositions(Transform botTransform, Transform playerTransform, float duration)
    {
        Vector3 botStartPosition = botTransform.position;
        Vector3 playerStartPosition = playerTransform.position;
        float elapsedTime = 0f;

        Rigidbody botRigidbody = botTransform.GetComponent<Rigidbody>();
        Rigidbody playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        bool botOriginalInterpolation = false;
        bool playerOriginalInterpolation = false;

        if (botRigidbody != null)
        {
            botOriginalInterpolation = botRigidbody.interpolation != RigidbodyInterpolation.None;
            botRigidbody.interpolation = RigidbodyInterpolation.None;
            botRigidbody.isKinematic = true;
        }

        if (playerRigidbody != null)
        {
            playerOriginalInterpolation = playerRigidbody.interpolation != RigidbodyInterpolation.None;
            playerRigidbody.interpolation = RigidbodyInterpolation.None;
            playerRigidbody.isKinematic = true;
        }

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            botTransform.position = Vector3.Lerp(botStartPosition, playerStartPosition, t);
            playerTransform.position = Vector3.Lerp(playerStartPosition, botStartPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        botTransform.position = playerStartPosition;
        playerTransform.position = botStartPosition;

        if (botRigidbody != null)
        {
            botRigidbody.velocity = Vector3.zero;
            botRigidbody.angularVelocity = Vector3.zero;
            botRigidbody.interpolation = botOriginalInterpolation ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            botRigidbody.isKinematic = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            playerRigidbody.interpolation = playerOriginalInterpolation ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            playerRigidbody.isKinematic = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BonusThird"))
        {
            canSelectBot = true;
            StartCoroutine(RespawnBonus(collision.gameObject));
        }
    }

    IEnumerator RespawnBonus(GameObject bonus)
    {
        Vector3 originalPosition = bonus.transform.position;
        Quaternion originalRotation = bonus.transform.rotation;
        Vector3 originalScale = bonus.transform.localScale;
        bonus.SetActive(false);

        yield return new WaitForSeconds(5.0f);

        bonus.transform.position = originalPosition;
        bonus.transform.rotation = originalRotation;
        bonus.transform.localScale = originalScale;
        bonus.SetActive(true);
    }
}
