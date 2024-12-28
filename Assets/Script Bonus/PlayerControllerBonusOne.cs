using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerControllerBonusOne : MonoBehaviour
{
    public float speed = 10.0f;
    public float maxSpeed = 5.0f;
    private Rigidbody rb;
    private bool canSelectPassThrough = false;
    private bool isPassingThroughWalls = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isActive = true;
    public AudioClip passThroughSound;
    [SerializeField] private AudioSource audioSource;
    public UIDocument uiDocument;
    private VisualElement bonus2Image;
    private VisualElement bonus3Image;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialScale = transform.localScale;

        

        var root = uiDocument.rootVisualElement;
        bonus2Image = root.Q<VisualElement>("Bonus2");
        bonus3Image = root.Q<VisualElement>("Bonus3");
    }

    void Update()
    {
        if (IsImageVisible(bonus2Image) || IsImageVisible(bonus3Image))
        {
            return;
        }

        if (canSelectPassThrough && Input.GetKeyDown(KeyCode.E))
        {
            if (passThroughSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(passThroughSound);
            }
            StartCoroutine(PassThroughWalls());
        }
    }

    public void EnablePassThrough()
    {
        canSelectPassThrough = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isActive && collision.gameObject.CompareTag("Igrok"))
        {
            StartCoroutine(RespawnAfterDelay());
            isActive = false;
        }

        if (collision.gameObject.CompareTag("Bonus"))
        {
            EnablePassThrough();
        }
    }

    IEnumerator RespawnAfterDelay()
    {
        gameObject.SetActive(false);

        yield return new WaitForSeconds(3f);
        gameObject.SetActive(true);
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        transform.localScale = initialScale;

        isActive = true;
    }

    IEnumerator PassThroughWalls()
    {
        canSelectPassThrough = false;
        isPassingThroughWalls = true;

        if (passThroughSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(passThroughSound);
        }

        int wallLayer = LayerMask.NameToLayer("Wall");

        if (wallLayer != -1)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, wallLayer, true);
            yield return new WaitForSeconds(5.0f);
            Physics.IgnoreLayerCollision(gameObject.layer, wallLayer, false);
        }
        else
        {
            Debug.LogError("Layer Wall does not exist.");
        }

        isPassingThroughWalls = false;
    }

    bool IsImageVisible(VisualElement imageElement)
    {
        return imageElement != null && imageElement.resolvedStyle.display != DisplayStyle.None;
    }
}