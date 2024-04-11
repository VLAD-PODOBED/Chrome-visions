using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public GameObject explosionPrefab; // Префаб взрыва

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car"))
        {
            // Создаем взрывный эффект в позиции столкновения
            Instantiate(explosionPrefab, collision.contacts[0].point, Quaternion.identity);

            // Уничтожаем объект, с которым столкнулись
            Destroy(collision.gameObject);

            // Обработка столкновения с машиной
            Debug.Log("Столкновение с машиной!");
        }
    }
}