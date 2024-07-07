using UnityEngine;
using System.Collections;

namespace DialogueSystem
{
    public class DialogueActivator : MonoBehaviour
    {
        [SerializeField] private float activationDelay = 4f; // Задержка перед активацией
        [SerializeField] private GameObject dialogueHolder; // Ссылка на объект с DialogueHolder

        private void Start()
        {
            // Сначала деактивируем DialogueHolder
            if (dialogueHolder != null)
            {
                dialogueHolder.SetActive(false);
            }

            // Запускаем корутину для активации с задержкой
            StartCoroutine(ActivateDialogueWithDelay());
        }

        private IEnumerator ActivateDialogueWithDelay()
        {
            // Ждем указанное время
            yield return new WaitForSeconds(activationDelay);

            // Активируем DialogueHolder
            if (dialogueHolder != null)
            {
                dialogueHolder.SetActive(true);
            }
            else
            {
                Debug.LogWarning("DialogueHolder не назначен в DialogueActivator!");
            }
        }
    }
}