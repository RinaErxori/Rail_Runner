using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad = "MainGameScene"; // Имя сцены для загрузки
    
    private Button startButton;
    
    void Start()
    {
        // Находим компонент Button на этом GameObject
        startButton = GetComponent<Button>();
        
        // Добавляем обработчик события нажатия кнопки
        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadTargetScene);
        }
        else
        {
            Debug.LogError("StartGameButton script requires a Button component!");
        }
    }
    
    void LoadTargetScene()
    {
        // Проверяем, является ли выбранная сцена сценой выхода
        if (sceneToLoad.Equals("ExitScene", System.StringComparison.OrdinalIgnoreCase))
        {
            // Выход из приложения
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else
        {
            // Загружаем указанную сцену
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    
    void OnDestroy()
    {
        // Важно удалять слушатели при уничтожении объекта
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(LoadTargetScene);
        }
    }
}