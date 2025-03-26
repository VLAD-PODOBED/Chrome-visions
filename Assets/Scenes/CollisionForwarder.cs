using UnityEngine;

public class CollisionForwarder : MonoBehaviour
{
    private Obsticle parentObsticle;

    void Awake()
    {
        // Получаем компонент Obsticle из родительского объекта
        parentObsticle = GetComponentInParent<Obsticle>();
        if (parentObsticle == null)
        {
            Debug.LogError("Не найден компонент Obsticle в родительском объекте!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (parentObsticle != null)
        {
            parentObsticle.OnChildCollisionEnter(collision);
        }
    }
}
