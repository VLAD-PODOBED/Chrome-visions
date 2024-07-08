using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerControllerBonusTwo : MonoBehaviour
{
    public float speed = 10.0f;
    public float maxSpeed = 5.0f;
    private Rigidbody rb;
    private bool canSelectBall = false;
    private bool selectionMode = false;
    private Rigidbody selectedBallRb;
    private Vector3 originalBallPosition;
    public AudioClip passThroughSound;
    private bool bonusActive = false;
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
            canSelectBall = true;
            collision.gameObject.GetComponent<BonusTwoRespawn>().StartRespawn();
        }
    }

    void Update()
    {
        if (IsImageVisible(bonus1Image) || IsImageVisible(bonus3Image))
        {
            Debug.Log("Cannot activate bonus because Bonus1 or Bonus3 image is visible.");
            return;
        }

        if (canSelectBall && Input.GetKeyDown(KeyCode.E) && !bonusActive)
        {
            if (passThroughSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(passThroughSound);
            }
            selectionMode = true;
            bonusActive = true;
            bonusEndTime = Time.time + 5f;
            Debug.Log("Selection mode activated");
        }

        if (bonusActive && Time.time > bonusEndTime)
        {
            canSelectBall = false;
            selectionMode = false;
            bonusActive = false;
            Debug.Log("Bonus deactivated after 5 seconds");
        }

        if (selectionMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && (hit.collider.CompareTag("BOT1") || hit.collider.CompareTag("BOT2") || hit.collider.CompareTag("BOT3")))
                {
                    Debug.Log("Bot selected");
                    selectedBallRb = hit.collider.GetComponent<Rigidbody>();
                    if (selectedBallRb != null)
                    {
                        originalBallPosition = selectedBallRb.transform.position;

                        Vector3 halfSubmergePosition = originalBallPosition - Vector3.up * 0.5f;
                        StartCoroutine(FreezeAndReturnBall(selectedBallRb, halfSubmergePosition, 5f));
                    }
                    canSelectBall = false;
                    selectionMode = false;
                    bonusActive = false;
                }
            }
        }
    }

    bool IsImageVisible(VisualElement imageElement)
    {
        return imageElement != null && imageElement.resolvedStyle.display != DisplayStyle.None;
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

        Debug.Log("Ball returned to original position");
    }
}