using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] private float delay = 20f; // Время в секундах перед уничтожением

    void Start()
    {
        // Вызываем метод DestroyObject через указанное время
        Invoke("DestroyObject", delay);
    }

    void DestroyObject()
    {
        // Уничтожаем объект
        Destroy(gameObject);
    }
}