using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class LapBonusOne : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement bonusImageContainer;
    private VisualElement bonus2Container;
    private VisualElement bonus3Container;
    private VisualElement buttonEContainer;
    private VisualElement iconContainer;
    private bool canHideBonus = false;
    private bool canHideButtonE = false;
    private bool canHideIcon = false;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        bonusImageContainer = root.Q<VisualElement>("Bonus1");
        bonus2Container = root.Q<VisualElement>("Bonus2");
        bonus3Container = root.Q<VisualElement>("Bonus3");
        buttonEContainer = root.Q<VisualElement>("ButtomE");
        iconContainer = root.Q<VisualElement>("icon");

        bonusImageContainer.style.display = DisplayStyle.None;
        buttonEContainer.style.display = DisplayStyle.None;
        iconContainer.style.display = DisplayStyle.None;
    }

    void Update()
    {
        if (canHideBonus && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(HideElementAfterDelay(bonusImageContainer, 0f));
            canHideBonus = false;
        }

        if (canHideButtonE && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(HideElementAfterDelay(buttonEContainer, 0f));
            canHideButtonE = false;
        }

        if (canHideIcon && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(HideElementAfterDelay(iconContainer, 0f));
            canHideIcon = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bonus"))
        {
            if (bonus2Container.style.display != DisplayStyle.Flex && bonus3Container.style.display != DisplayStyle.Flex)
            {
                ShowElement(bonusImageContainer);
                canHideBonus = true;
                ShowElement(buttonEContainer);
                canHideButtonE = true;
                ShowElement(iconContainer);
                canHideIcon = true;
            }
        }
    }

    void ShowElement(VisualElement element)
    {
        if (element.style.display == DisplayStyle.None)
        {
            element.style.display = DisplayStyle.Flex;
        }
    }

    IEnumerator HideElementAfterDelay(VisualElement element, float delay)
    {
        yield return new WaitForSeconds(delay);
        element.style.display = DisplayStyle.None;
    }
}
