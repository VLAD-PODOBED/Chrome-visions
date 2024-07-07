using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "chrome_visions";
        [SerializeField] private float initialDelay = 5f; // Добавляем настраиваемую задержку
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            StartCoroutine(ShowObjectWithDelay(initialDelay));
        }

        private IEnumerator ShowObjectWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.localScale = originalScale;
            StartCoroutine(dialogueSequence());
        }

        private IEnumerator dialogueSequence()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Deactivate();
                var currentLine = transform.GetChild(i).GetComponent<DialogueLine>();
                currentLine.gameObject.SetActive(true);
                currentLine.StartDialogue();
                yield return new WaitUntil(() => currentLine.finished);
            }
            LoadNextScene();
        }

        private void Deactivate()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private IEnumerator LoadSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(0.1f);
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        private void LoadNextScene()
        {
            StartCoroutine(LoadSceneAsync());
        }
    }
}