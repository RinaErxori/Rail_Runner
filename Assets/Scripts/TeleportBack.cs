using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public float backTo = 20f;
    [Header("Настройки триггера")]
    public Collider triggerCollider;
    public string playerTag = "Player";

    private void Start()
    {
        // Проверка и настройка триггера
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
            else
            {
                Debug.LogError("Нет коллайдера на объекте!", gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            TeleportPlayer(other.transform);
        }
    }

    void TeleportPlayer(Transform playerTransform)
    {
        // Перемещаем игрока на 20 единиц по оси X
        playerTransform.position = new Vector3(
            playerTransform.position.x - backTo,
            playerTransform.position.y,
            playerTransform.position.z
        );
    }
}
