using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // ��������� ��� ���������

public class DeadlyObstacle : MonoBehaviour
{
    [Header("��������� ������")]
    [SerializeField] private float restartDelay = 2f;
    [SerializeField] private bool debugLogs = true;
    [Tooltip("����� �������, ���� �������� �� � ���� �������")]
    [SerializeField] private Vector3 safeDirection = Vector3.up;
    [SerializeField] private float safeAngle = 45f;

    [Header("������� ������")]
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private AudioClip deathSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Vector3 contactNormal = collision.contacts[0].normal;
        float angle = Vector3.Angle(contactNormal, safeDirection);

        if (angle > safeAngle && angle < 180f)
        {
            if (debugLogs) Debug.Log($"����� �������� ������� �������! ����: {angle}�");
            StartCoroutine(KillPlayer(collision.gameObject)); // ���������� KillPlayer -> PillPlayer
        }
        else if (debugLogs)
        {
            Debug.Log($"����� �������� ���������� �������. ����: {angle}�");
        }
    }

    private IEnumerator KillPlayer(GameObject player) // ���������� ��� ������
    {
        // �������� ���������� ������
        Rigidbody rb = player.GetComponent<Rigidbody>();
        MonoBehaviour[] movementScripts = player.GetComponents<MonoBehaviour>();
        Collider col = player.GetComponent<Collider>();

        // ������������� ������
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // ��������� ���������
        if (col != null) col.enabled = false;

        // ��������� ��� ������� ��������
        foreach (var script in movementScripts)
        {
            if (script != this && script.enabled)
            {
                script.enabled = false;
            }
        }

        // ������������� ������� ������
        PlayDeathEffects(player.transform.position);

        if (debugLogs) Debug.Log($"����� ����. ������������ ����� {restartDelay} ���.");

        // ���� ����� �������������
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