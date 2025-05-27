using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private SmartLaneRunner player;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private bool debugLogs = true;

    private void Awake()
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("GameOverPanel не назначен в GameOverUI!", this);
        }
        if (retryButton == null)
        {
            Debug.LogError("RetryButton не назначен в GameOverUI!", this);
        }
        if (menuButton == null)
        {
            Debug.LogError("MenuButton не назначен в GameOverUI!", this);
        }
        if (player == null)
        {
            Debug.LogError("SmartLaneRunner не назначен в GameOverUI!", this);
        }
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("MainMenuSceneName не задано в GameOverUI!", this);
        }
    }

    private void Start()
    {
        // Убедимся, что объект активен
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("GameOverUI объект не активен в иерархии, подписка может не сработать!", this);
        }

        // Подписываемся на событие
        if (player != null)
        {
            player.OnGameOver -= ShowGameOverPanel; // Отписываемся на случай дублирования
            player.OnGameOver += ShowGameOverPanel;
            if (debugLogs) Debug.Log("GameOverUI подписан на событие OnGameOver");
        }

        // Скрываем панель при старте
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverPanel не найден, не могу скрыть панель!", this);
        }

        // Назначаем обработчики для кнопок
        if (retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(OnRetryButtonClicked);
        }
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(OnMenuButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnGameOver -= ShowGameOverPanel;
            if (debugLogs) Debug.Log("GameOverUI отписан от события OnGameOver");
        }
    }

    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (debugLogs) Debug.Log("GameOverPanel активирован");
        }
        else
        {
            Debug.LogError("GameOverPanel не назначен, не могу показать панель!", this);
        }
    }

    private void OnRetryButtonClicked()
    {
        if (player != null)
        {
            player.RestartLevel();
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
                if (debugLogs) Debug.Log("Уровень перезапущен, GameOverPanel скрыт");
            }
        }
        else
        {
            Debug.LogError("SmartLaneRunner не назначен, не могу перезапустить уровень!", this);
        }
    }

    private void OnMenuButtonClicked()
    {
        Time.timeScale = 1f;
        if (debugLogs) Debug.Log($"Загрузка сцены главного меню: {mainMenuSceneName}");
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
}