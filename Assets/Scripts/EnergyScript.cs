using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [Header("Energy Settings")]
    [Tooltip("Количество восстанавливаемой энергии")]
    [SerializeField] private float energyAmount = 25f;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрок
        if (other.CompareTag("Player"))
        {
            // Пытаемся получить компонент SmartLaneRunner у игрока
            SmartLaneRunner player = other.GetComponent<SmartLaneRunner>();

            if (player != null)
            {
                // Восстанавливаем энергию игроку
                player.AddEnergy(energyAmount);

                // Воспроизводим эффекты
                PlayPickupEffects();

                // Отключаем объект (если он должен исчезнуть после подбора)
                gameObject.SetActive(false);

                Debug.Log($"Игрок подобрал {energyAmount} единиц энергии");
            }
            else
            {
                Debug.LogWarning("У игрока не найден компонент SmartLaneRunner!");
            }
        }
    }

    private void PlayPickupEffects()
    {
        // Воспроизведение партиклов
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        // Воспроизведение звука
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
    }

    // Метод для настройки количества энергии через другие скрипты
    public void SetEnergyAmount(float amount)
    {
        energyAmount = amount;
    }
}