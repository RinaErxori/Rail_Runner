using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishTrigger : MonoBehaviour
{
    [Header("���������")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float finishDelay = 1f;
    [SerializeField] private string nextSceneName;

    [Header("�������")]
    [SerializeField] private ParticleSystem winEffect;
    [SerializeField] private AudioClip winSound;

    private bool isTriggered = false;
    private Rigidbody playerRigidbody;
    private MonoBehaviour[] playerMovementScripts;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag(playerTag))
        {
            isTriggered = true;
            Debug.Log("������! ����� ������ ������");

            // �������� ���������� ������
            playerRigidbody = other.GetComponent<Rigidbody>();
            playerMovementScripts = other.GetComponents<MonoBehaviour>();

            // ������������� ������
            StopPlayerMovement();

            // ������������� �������
            PlayWinEffects();

            // ��������� ������� � ���������
            Invoke("CompleteLevel", finishDelay);
        }
    }

    private void StopPlayerMovement()
    {
        // ������������� ���������� ��������
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        // ��������� ��� ������� ��������
        foreach (var script in playerMovementScripts)
        {
            if (script != this && script is not LevelFinishTrigger)
            {
                script.enabled = false;
            }
        }
    }

    private void PlayWinEffects()
    {
        if (winEffect != null)
            Instantiate(winEffect, transform.position, Quaternion.identity);

        if (winSound != null)
            AudioSource.PlayClipAtPoint(winSound, transform.position);
    }

    private void CompleteLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("�� ������� �������� ��������� �����!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}