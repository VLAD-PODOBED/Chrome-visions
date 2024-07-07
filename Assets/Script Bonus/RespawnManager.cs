using UnityEngine;
using System.Collections;

public class RespawnManager : MonoBehaviour
{
    public void RespawnObject(GameObject obj, Vector3 position, Quaternion rotation, Vector3 scale, float respawnTime)
    {
        StartCoroutine(RespawnCoroutine(obj, position, rotation, scale, respawnTime));
    }

    private IEnumerator RespawnCoroutine(GameObject obj, Vector3 position, Quaternion rotation, Vector3 scale, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        obj.transform.position = position; 
        obj.transform.rotation = rotation; 
        obj.transform.localScale = scale; 
        obj.SetActive(true);  
    }
}
