using UnityEngine;
using TMPro;

public class SimpleTextTrigger : MonoBehaviour
{
    [Header("Основные настройки")]
    public TMP_Text textElement; // Перетащите сюда ваш TextMeshPro из Canvas
    public string message = "Ваш текст здесь"; // Текст для отображения
    public float showTime = 3f; // Время показа в секундах

    [Header("Настройки триггера")]
    public Collider triggerCollider; // Перетащите сюда коллайдер-триггер
    public string playerTag = "Player"; // Тег игрока

    private void Start()
    {
        // Проверяем и выключаем текст в начале игры
        if (textElement != null)
        {
            textElement.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Не назначен TextMeshPro элемент!", gameObject);
        }

        // Проверяем триггер
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
            if (triggerCollider == null)
            {
                Debug.LogError("Нет коллайдера на объекте!", gameObject);
            }
            else
            {
                triggerCollider.isTrigger = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ShowText();
        }
    }

    void ShowText()
    {
        if (textElement == null) return;

        textElement.text = message;
        textElement.gameObject.SetActive(true);

        // Выключаем текст через заданное время
        Invoke("HideText", showTime);
    }

    void HideText()
    {
        if (textElement != null)
        {
            textElement.gameObject.SetActive(false);
        }
    }
}