using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadlyObstacle : MonoBehaviour
{
    [Header("Настройки смерти")]
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private bool debugLogs = true;
    [Tooltip("Игрок умирает, если касается НЕ с этой стороны")]
    [SerializeField] private Vector3 safeDirection = Vector3.up; // Безопасное направление (верх)
    [SerializeField] private float safeAngle = 45f; // Допустимый угол для "безопасного" касания

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // Проверяем, с какой стороны игрок коснулся объекта
        Vector3 contactNormal = collision.contacts[0].normal; // Нормаль точки контакта
        float angle = Vector3.Angle(contactNormal, safeDirection);

        if (angle > safeAngle) // Если угол вне безопасной зоны
        {
            if (debugLogs) Debug.Log($"Игрок коснулся опасной стороны! Угол: {angle}°");
            KillPlayer(collision.gameObject);
        }
        else if (debugLogs)
        {
            Debug.Log($"Игрок коснулся безопасной стороны. Угол: {angle}°");
        }
    }

    private void KillPlayer(GameObject player)
    {
        player.SetActive(false);
        if (debugLogs) Debug.Log($"Игрок умер. Перезагрузка через {restartDelay} сек.");
        Invoke(nameof(RestartLevel), restartDelay);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Визуализация безопасного направления в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, safeDirection.normalized * 2f);
        Gizmos.DrawWireSphere(transform.position + safeDirection.normalized * 2f, 0.2f);
    }
}