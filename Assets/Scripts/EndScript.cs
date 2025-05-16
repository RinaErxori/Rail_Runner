using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishTrigger : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float finishDelay = 1f;
    [SerializeField] private string nextSceneName;

    [Header("Эффекты")]
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
            Debug.Log("Победа! Игрок достиг финиша");

            // Получаем компоненты игрока
            playerRigidbody = other.GetComponent<Rigidbody>();
            playerMovementScripts = other.GetComponents<MonoBehaviour>();

            // Останавливаем игрока
            StopPlayerMovement();

            // Воспроизводим эффекты
            PlayWinEffects();

            // Завершаем уровень с задержкой
            Invoke("CompleteLevel", finishDelay);
        }
    }

    private void StopPlayerMovement()
    {
        // Останавливаем физическое движение
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.isKinematic = true;
        }

        // Отключаем все скрипты движения
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
            Debug.LogWarning("Не указано название следующей сцены!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}