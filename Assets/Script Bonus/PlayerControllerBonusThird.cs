using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerControllerBonusThird : MonoBehaviour
{
    private bool canSelectBot = false;
    private bool isSelectionMode = false;
    private Transform selectedBotTransform;
    public AudioClip passThroughSound;
    private AudioSource audioSource;
    private bool bonusActive = false;
    private float bonusEndTime;

    public UIDocument uiDocument;
    private VisualElement potImage;
    private VisualElement topImage;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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
        if (IsImageVisible(potImage) || IsImageVisible(topImage))
        {
            Debug.Log("Cannot activate bonus because pot or top image is visible.");
            return;
        }

        if (canSelectBot && Input.GetKeyDown(KeyCode.E) && !bonusActive)
        {
            isSelectionMode = true;
            bonusActive = true;
            bonusEndTime = Time.time + 5f;
            Debug.Log("Selection mode activated");
        }

        if (bonusActive && Time.time > bonusEndTime)
        {
            canSelectBot = false;
            isSelectionMode = false;
            bonusActive = false;
            Debug.Log("Bonus deactivated after 5 seconds");
        }

        if (isSelectionMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && (hit.collider.CompareTag("BOT1") || hit.collider.CompareTag("BOT2") || hit.collider.CompareTag("BOT3")))
                {
                    if (passThroughSound != null && audioSource != null)
                    {
                        audioSource.PlayOneShot(passThroughSound);
                    }
                    selectedBotTransform = hit.collider.transform;
                    StartCoroutine(SwapPositions(selectedBotTransform, transform, 0.5f));
                    canSelectBot = false;
                    isSelectionMode = false;
                    bonusActive = false;
                }
            }
        }
    }

    bool IsImageVisible(VisualElement imageElement)
    {
        return imageElement != null && imageElement.resolvedStyle.display != DisplayStyle.None;
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