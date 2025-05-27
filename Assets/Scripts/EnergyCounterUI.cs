using UnityEngine;
using TMPro; // Используется для TextMeshPro, если используете обычный Text, замените на UnityEngine.UI

public class EnergyCounterUI : MonoBehaviour
{
    [SerializeField] private SmartLaneRunner player; // Ссылка на компонент SmartLaneRunner
    [SerializeField] private TextMeshProUGUI energyText; // Ссылка на UI текст

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("SmartLaneRunner не назначен в EnergyCounterUI!");
        }
        if (energyText == null)
        {
            Debug.LogError("TextMeshProUGUI не назначен в EnergyCounterUI!");
        }
    }

    private void Update()
    {
        if (player != null && energyText != null)
        {
            // Обновляем текст с текущей энергией
            float currentEnergy = player.GetCurrentEnergy(); // Предполагается, что добавлен геттер
            float maxEnergy = player.GetMaxEnergy(); // Предполагается, что добавлен геттер
            energyText.text = $"Энергия: {currentEnergy:F1}/{maxEnergy:F0}";
        }
    }
}