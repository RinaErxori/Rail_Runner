using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Добавляем эту директиву

public class DeadlyObstacle : MonoBehaviour
{
    [Header("Настройки смерти")]
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private bool debugLogs = true;
    [Tooltip("Игрок умирает, если касается НЕ с этой стороны")]
    [SerializeField] private Vector3 safeDirection = Vector3.up;
    [SerializeField] private float safeAngle = 45f;

    [Header("Эффекты смерти")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private AudioClip deathSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Vector3 contactNormal = collision.contacts[0].normal;
        float angle = Vector3.Angle(contactNormal, safeDirection);

        if (angle > safeAngle && angle < 180f)
        {
            if (debugLogs) Debug.Log($"Игрок коснулся опасной стороны! Угол: {angle}°");
            StartCoroutine(KillPlayer(collision.gameObject)); // Исправлено KillPlayer -> PillPlayer
        }
        else if (debugLogs)
        {
            Debug.Log($"Игрок коснулся безопасной стороны. Угол: {angle}°");
        }
    }

    private IEnumerator KillPlayer(GameObject player) // Исправлено имя метода
    {
        // Получаем компоненты игрока
        Rigidbody rb = player.GetComponent<Rigidbody>();
        MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
        Collider col = player.GetComponent<Collider>();

        // Останавливаем игрока
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Отключаем коллайдер
        if (col != null) col.enabled = false;

        // Отключаем все скрипты движения
        foreach (var script in movementScripts)
        {
            if (script != this && script.enabled)
            {
                script.enabled = false;
            }
        }

        // Воспроизводим эффекты смерти
        PlayDeathEffects(player.transform.position);

        if (debugLogs) Debug.Log($"Игрок умер. Перезагрузка через {restartDelay} сек.");

        // Ждем перед перезагрузкой
        yield return new WaitForSeconds(restartDelay);
        RestartLevel();
    }

    private void PlayDeathEffects(Vector3 position)
    {
        if (deathEffect != null)
            Instantiate(deathEffect, position, Quaternion.identity);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, position);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, safeDirection.normalized * 2f);
        Gizmos.DrawWireSphere(transform.position + safeDirection.normalized * 2f, 0.2f);
    }
}