using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RestartScene();
        }
    }

    public void RestartScene()
    {
        string sceneName = "Chrome_visions";
        SceneManager.LoadScene(sceneName);
    }
}
