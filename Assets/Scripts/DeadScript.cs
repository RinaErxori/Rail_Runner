using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Необходимо для корутин

public class DeadlyObstacle : MonoBehaviour
{
    [Header("Настройки столкновения")]
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private bool debugLogs = true;
    [Tooltip("Направление, с которого удар не будет считаться смертельным")]
    [SerializeField] private Vector3 safeDirection = Vector3.up;
    [SerializeField]  float safeAngleFrom = 45f;
    [SerializeField]  float safeAngleTo = 180f;

    [Header("Эффекты смерти")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private AudioClip deathSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Vector3 contactNormal = collision.contacts[0].normal;
        float angle = Vector3.Angle(contactNormal, safeDirection);

        if (angle > safeAngleFrom && angle < safeAngleTo)
        {
            if (debugLogs) Debug.Log($"Объект столкнулся под смертельным углом! Угол: {angle}°");
            StartCoroutine(KillPlayer(collision.gameObject));
        }
        else if (debugLogs)
        {
            Debug.Log($"Объект столкнулся под безопасным углом. Угол: {angle}°");
        }
    }

    private IEnumerator KillPlayer(GameObject player)
    {
        // Получаем компоненты игрока
        Rigidbody rb = player.GetComponent<Rigidbody>();
        MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
        Collider col = player.GetComponent<Collider>();

        // Останавливаем физику
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