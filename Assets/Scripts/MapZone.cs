using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorDetectorUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite redSprite;    // Спрайт для красно-оранжевого цвета
    [SerializeField] private Sprite greenSprite;  // Спрайт для зеленовато-бирюзового цвета
    [SerializeField] private Sprite orangeSprite; // Спрайт для оранжево-красного цвета
    [SerializeField] private Sprite blueSprite;   // Спрайт для голубого цвета
    [SerializeField] private Sprite purpleSprite; // Спрайт для фиолетово-розового цвета

    private Image uiImage;
    private Texture2D targetTexture;
    private RectTransform rectTransform;

    // Цвета для проверки
    private readonly Color redColor = new Color(0.996f, 0.141f, 0.090f);
    private readonly Color greenColor = new Color(0.059f, 0.710f, 0.325f);
    private readonly Color orangeColor = new Color(1.000f, 0.522f, 0.094f);
    private readonly Color blueColor = new Color(0.000f, 0.490f, 0.784f, 1.000f);
    private readonly Color purpleColor = new Color(0.765f, 0.451f, 0.765f, 1.000f); // Заменён на RGBA(0.765, 0.451, 0.765, 1.000)
    private readonly float colorThreshold = 0.05f; // Порог для сравнения цветов

    void Start()
    {
        // Получаем компоненты
        uiImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        if (uiImage == null || uiImage.sprite == null)
        {
            Debug.LogError("Image или Sprite не найдены!");
            return;
        }

        // Получаем текстуру спрайта
        targetTexture = uiImage.sprite.texture;
        if (targetTexture == null)
        {
            Debug.LogError("Текстура не найдена! Убедитесь, что Read/Write включен.");
        }

        // Проверка, назначены ли спрайты
        if (redSprite == null || greenSprite == null || orangeSprite == null || 
            blueSprite == null || purpleSprite == null)
        {
            Debug.LogWarning("Один или несколько спрайтов не назначены в инспекторе!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Получаем локальные координаты клика
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Нормализуем координаты в диапазон текстуры
            float u = (localPoint.x / rectTransform.rect.width + 0.5f) * targetTexture.width;
            float v = (localPoint.y / rectTransform.rect.height + 0.5f) * targetTexture.height;

            // Проверяем, что координаты в пределах текстуры
            if (u >= 0 && u < targetTexture.width && v >= 0 && v < targetTexture.height)
            {
                Color pixelColor = targetTexture.GetPixel((int)u, (int)v);
                
                // Выводим информацию о цвете в дебаг
                Debug.Log($"Цвет пикселя: {pixelColor} (R: {pixelColor.r}, G: {pixelColor.g}, B: {pixelColor.b}, A: {pixelColor.a})");

                // Игнорируем прозрачные пиксели
                if (pixelColor.a < 0.1f)
                {
                    Debug.Log("Прозрачный пиксель!");
                    return;
                }

                // Сравниваем цвет пикселя с целевыми цветами
                if (IsColorMatch(pixelColor, redColor))
                {
                    uiImage.sprite = redSprite;
                    Debug.Log("Обнаружен яркий красно-оранжевый цвет! Спрайт изменён.");
                }
                else if (IsColorMatch(pixelColor, greenColor))
                {
                    uiImage.sprite = greenSprite;
                    Debug.Log("Обнаружен тёмный зеленовато-бирюзовый цвет! Спрайт изменён.");
                }
                else if (IsColorMatch(pixelColor, orangeColor))
                {
                    uiImage.sprite = orangeSprite;
                    Debug.Log("Обнаружен яркий оранжево-красный цвет! Спрайт изменён.");
                }
                else if (IsColorMatch(pixelColor, blueColor))
                {
                    uiImage.sprite = blueSprite;
                    Debug.Log("Обнаружен голубой цвет! Спрайт изменён.");
                }
                else if (IsColorMatch(pixelColor, purpleColor))
                {
                    uiImage.sprite = purpleSprite;
                    Debug.Log("Обнаружен фиолетово-розовый цвет! Спрайт изменён.");
                }
                else
                {
                    Debug.Log("Цвет не является ни красно-оранжевым, ни зеленовато-бирюзовым, ни оранжево-красным, ни голубым, ни фиолетово-розовым.");
                }
            }
            else
            {
                Debug.LogWarning("Клик вне текстуры!");
            }
        }
    }

    private bool IsColorMatch(Color a, Color b)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < colorThreshold;
    }
}