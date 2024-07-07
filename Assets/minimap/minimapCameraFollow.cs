using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public RectTransform playerInMap; // Иконка игрока на миникарте
    public RectTransform map2dEnd; // Край карты в 2D
    public Transform map3dParent; // Родитель карты в 3D
    public Transform map3dEnd; // Край карты в 3D

    private Vector3 normalized, mapped;

    private void Update()
    {
        // Нормализуем позицию игрока относительно карты
        normalized = Divide(
                map3dParent.InverseTransformPoint(this.transform.position),
                map3dEnd.position - map3dParent.position
            );

        normalized.y = normalized.z;
        mapped = Multiply(normalized, map2dEnd.localPosition);
        mapped.z = 0;

        // Обновляем позицию иконки игрока на миникарте
        playerInMap.localPosition = mapped;

        // Обновляем вращение иконки игрока на миникарте
        UpdatePlayerIconRotation();
    }

    private void UpdatePlayerIconRotation()
    {
        // Получаем угол поворота игрока вокруг оси Y
        float playerRotationY = transform.eulerAngles.y;

        // Применяем этот угол к иконке игрока на миникарте
        playerInMap.localRotation = Quaternion.Euler(0, 0, -playerRotationY);
    }

    private static Vector3 Divide(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    private static Vector3 Multiply(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}
