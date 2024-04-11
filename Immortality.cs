using UnityEngine;

public class CarCollision : MonoBehaviour
{
    public float invincibilityDuration = 3f; // Длительность временного бессмертия при касании с машиной

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Получаем компонент управления игроком
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Включаем временное бессмертие игрока
                playerController.ActivateInvincibility(invincibilityDuration);

                // Обработка столкновения с машиной
                Debug.Log("Столкновение с машиной! Временное бессмертие игрока.");
            }
        }
    }
}