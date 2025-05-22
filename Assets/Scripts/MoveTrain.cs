using UnityEngine;

public class TriggerPrefabSpawner : MonoBehaviour
{
    public string playerTag = "Player"; // Тег игрока
    public GameObject prefab; // Префаб, который будет появляться
    public GameObject strip; // Объект полосы, на которой появится префаб
    public float HowFar = 40f; // Смещение по X

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            SpawnPrefab();
        }
    }

    void SpawnPrefab()
    {
        // Проверяем, что полоса задана
        if (strip == null)
        {
            Debug.LogError("Полоса не задана!");
            return;
        }

        // Создаем префаб на полосе с учетом смещения
        Vector3 spawnPosition = new Vector3(
            transform.position.x - HowFar,
            strip.transform.position.y + 4f,
            strip.transform.position.z
        );

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}