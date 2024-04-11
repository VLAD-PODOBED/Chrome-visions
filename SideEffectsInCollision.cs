using UnityEngine;

public class CarCollision : MonoBehaviour
{
    // Вызывается при столкновении с другим коллайдером
    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, является ли объект, с которым произошло столкновение, машиной
        if (collision.gameObject.CompareTag("Car"))
        {
            // Обработка столкновения с машиной
            Debug.Log("Столкновение с машиной!");

            // Пример получения дополнительной информации о столкновении
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white, 2, false);
            }
        }
    }
}