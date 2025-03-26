using System.Collections;
using UnityEngine;

public class Obsticle : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 175f;
    private bool canCollide = true;
    private float collisionCooldown = 2f;

    void Update()
    {
        // Вращение препятствия вокруг своей оси
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }

    // Метод для обработки столкновений, поступающих от дочерних коллайдеров
    public void OnChildCollisionEnter(Collision collision)
    {
        if (!canCollide)
            return;

        // Игнорируем столкновения с триггер-коллайдерами
        if (collision.collider.isTrigger)
            return;

        Debug.Log("Физическое столкновение обнаружено с: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Igrok"))
        {
            IgrokController igrokController = collision.gameObject.GetComponent<IgrokController>();
            if (igrokController != null)
            {
                StartCoroutine(igrokController.TeleportToRoadAfterDelayOnRoad());
            }
            else
            {
                Debug.LogWarning("IgrokController не найден на столкнувшемся объекте.");
            }
        }

        StartCoroutine(CollisionCooldown());
    }

    // Если родительский объект сам получает OnCollisionEnter, обрабатываем его так же
    private void OnCollisionEnter(Collision collision)
    {
        OnChildCollisionEnter(collision);
    }

    private IEnumerator CollisionCooldown()
    {
        canCollide = false;
        yield return new WaitForSeconds(collisionCooldown);
        canCollide = true;
    }
}
