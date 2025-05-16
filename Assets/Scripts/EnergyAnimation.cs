using UnityEngine;

public class SimpleItemAnimation : MonoBehaviour
{
    [Header("Настройки вращения")]
    [SerializeField] private float rotationSpeed = 90f; // Скорость вращения
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // Ось вращения (Y)

    [Header("Настройки плавания")]
    [SerializeField] private float floatHeight = 0.5f; // Высота плавания
    [SerializeField] private float floatSpeed = 1f; // Скорость плавания

    private Vector3 startPosition;

    private void Start()
    {
        // Запоминаем стартовую позицию
        startPosition = transform.position;
    }

    private void Update()
    {
        // Вращение объекта
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

        // Плавное движение вверх-вниз
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}