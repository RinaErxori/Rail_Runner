using UnityEngine;

public class DeadlyObstacle : MonoBehaviour
{
    [Header("Настройки столкновения")]
    [SerializeField] private bool debugLogs = true;
    [Tooltip("Направление, с которого удар не будет считаться смертельным")]
    [SerializeField] private Vector3 safeDirection = Vector3.up;
    [SerializeField] private float safeAngleFrom = 45f;
    [SerializeField] private float safeAngleTo = 180f;

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
            KillPlayer(collision.gameObject);
        }
        else if (debugLogs)
        {
            Debug.Log($"Объект столкнулся под безопасным углом. Угол: {angle}°");
        }
    }

    private void KillPlayer(GameObject player)
    {
        // Получаем компонент SmartLaneRunner
        SmartLaneRunner runner = player.GetComponent<SmartLaneRunner>();
        if (runner != null)
        {
            // Воспроизводим эффекты смерти
            PlayDeathEffects(player.transform.position);
            // Вызываем метод смерти в SmartLaneRunner
            runner.KillPlayer();
        }
        else
        {
            if (debugLogs) Debug.LogError("SmartLaneRunner не найден на объекте игрока!");
        }
    }

    private void PlayDeathEffects(Vector3 position)
    {
        if (deathEffect != null)
            Instantiate(deathEffect, position, Quaternion.identity);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, safeDirection.normalized * 2f);
        Gizmos.DrawWireSphere(transform.position + safeDirection.normalized * 2f, 0.2f);
    }
}