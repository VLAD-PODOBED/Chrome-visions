using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArchCollisionTracker : MonoBehaviour
{
    private Dictionary<string, int> collisionCounts = new Dictionary<string, int>
    {
        { "BOT1", 0 },
        { "BOT2", 0 },
        { "BOT3", 0 }
    };

    void OnCollisionEnter(Collision collision)
    {
        GameObject collidedObject = collision.gameObject;
        string tag = collidedObject.tag;

        if (collisionCounts.ContainsKey(tag))
        {
            collisionCounts[tag]++;

            Debug.Log($"{tag} has collided with arch {collisionCounts[tag]} times.");

            if (collisionCounts[tag] == 3)
            {
                LoadGameOverScene();
            }
        }
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}
