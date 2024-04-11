using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public float speedBoost = 10f; // Увеличение скорости при касании с машиной

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Получаем компонент управления игроком
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Увеличиваем скорость игрока
                playerController.IncreaseSpeed(speedBoost);

                // Обработка столкновения с машиной
                Debug.Log("Столкновение с машиной! Увеличение скорости игрока.");
            }
        }
    }
}