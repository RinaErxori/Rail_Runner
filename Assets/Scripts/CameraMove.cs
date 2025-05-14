using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Ссылка на трансформ игрока
    [SerializeField] private float followSpeed = 5f; // Скорость следования камеры

    [Header("Position Settings")]
    [SerializeField] private float yPosition = 10f; // Фиксированная позиция Y
    [SerializeField] private float zPosition = -10f; // Фиксированная позиция Z
    [SerializeField] private float xofset = 10f; // Фиксированная позиция Z

    private Vector3 _targetPosition;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned for CameraFollowX!");
            enabled = false;
            return;
        }

        // Инициализируем начальную позицию камеры
        _targetPosition = new Vector3(target.position.x+xofset, yPosition, zPosition);
        transform.position = _targetPosition;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Обновляем только X-координату, сохраняя Y и Z
        _targetPosition.x = target.position.x+xofset;

        // Плавное перемещение камеры
        transform.position = Vector3.Lerp(transform.position, _targetPosition, followSpeed * Time.deltaTime);
    }

    // Метод для ручной установки позиции Y и Z (если нужно изменить во время игры)
    public void SetCameraPosition(float newY, float newZ)
    {
        yPosition = newY;
        zPosition = newZ;
        _targetPosition.y = newY;
        _targetPosition.z = newZ;
    }
}