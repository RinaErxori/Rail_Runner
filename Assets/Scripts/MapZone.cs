using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ColorDetectorUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite redSprite;    // Спрайт для красно-оранжевого цвета
    [SerializeField] private Sprite greenSprite;  // Спрайт для зеленовато-бирюзового цвета
    [SerializeField] private Sprite orangeSprite; // Спрайт для оранжево-красного цвета
    [SerializeField] private Sprite blueSprite;   // Спрайт для голубого цвета
    [SerializeField] private Sprite purpleSprite; // Спрайт для фиолетово-розового цвета
    [SerializeField] private Image secondaryImage; // Вторая Image для перехода
    [SerializeField] private float fadeDuration = 0.5f; // Длительность перехода

    private Image uiImage;
    private Texture2D targetTexture;
    private RectTransform rectTransform;
    private bool isFading = false;

    // Цвета для проверки
    private readonly Color redColor = new Color(0.996f, 0.141f, 0.090f);
    private readonly Color greenColor = new Color(0.059f, 0.710f, 0.325f);
    private readonly Color orangeColor = new Color(1.000f, 0.522f, 0.094f);
    private readonly Color blueColor = new Color(0.000f, 0.490f, 0.784f, 1.000f);
    private readonly Color purpleColor = new Color(0.765f, 0.451f, 0.765f, 1.000f);
    private readonly float colorThreshold = 0.05f; // Порог для сравнения цветов

    void Start()
    {
        // Получаем компоненты
        uiImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Проверка основных компонентов
        if (uiImage == null || uiImage.sprite == null)
        {
            Debug.LogError("Основная Image или Sprite не найдены!");
            enabled = false;
            return;
        }

        if (secondaryImage == null)
        {
            Debug.LogError("Вторичная Image не назначена!");
            enabled = false;
            return;
        }

        // Инициализируем вторую Image
        secondaryImage.sprite = uiImage.sprite;
        secondaryImage.color = new Color(1f, 1f, 1f, 0f); // Прозрачная изначально

        // Получаем текстуру спрайта
        targetTexture = uiImage.sprite.texture;
        if (targetTexture == null)
        {
            Debug.LogError("Текстура не найдена! Убедитесь, что Read/Write включен.");
            enabled = false;
            return;
        }

        // Проверка назначения всех спрайтов
        if (redSprite == null || greenSprite == null || orangeSprite == null || 
            blueSprite == null || purpleSprite == null)
        {
            Debug.LogWarning("Один или несколько спрайтов не назначены в инспекторе!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFading) return; // Не обрабатываем клики во время перехода

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
                
                Debug.Log($"Цвет пикселя: {pixelColor} (R: {pixelColor.r}, G: {pixelColor.g}, B: {pixelColor.b}, A: {pixelColor.a})");

                // Игнорируем прозрачные пиксели
                if (pixelColor.a < 0.1f)
                {
                    Debug.Log("Прозрачный пиксель!");
                    return;
                }

                // Проверяем соответствие цвета и выбираем спрайт
                Sprite newSprite = null;
                string colorName = "";
                if (IsColorMatch(pixelColor, redColor) && redSprite != null)
                {
                    newSprite = redSprite;
                    colorName = "яркий красно-оранжевый";
                }
                else if (IsColorMatch(pixelColor, greenColor) && greenSprite != null)
                {
                    newSprite = greenSprite;
                    colorName = "тёмный зеленовато-бирюзовый";
                }
                else if (IsColorMatch(pixelColor, orangeColor) && orangeSprite != null)
                {
                    newSprite = orangeSprite;
                    colorName = "яркий оранжево-красный";
                }
                else if (IsColorMatch(pixelColor, blueColor) && blueSprite != null)
                {
                    newSprite = blueSprite;
                    colorName = "голубой";
                }
                else if (IsColorMatch(pixelColor, purpleColor) && purpleSprite != null)
                {
                    newSprite = purpleSprite;
                    colorName = "фиолетово-розовый";
                }

                // Если спрайт найден, запускаем переход
                if (newSprite != null)
                {
                    Debug.Log($"Обнаружен {colorName} цвет! Спрайт изменён.");
                    StartCoroutine(FadeToNewSprite(newSprite));
                }
                else
                {
                    Debug.Log("Цвет не распознан или спрайт не назначен.");
                }
            }
            else
            {
                Debug.LogWarning("Клик вне текстуры!");
            }
        }
    }

    private IEnumerator FadeToNewSprite(Sprite newSprite)
    {
        isFading = true;

        // Устанавливаем новый спрайт на вторую Image
        secondaryImage.sprite = newSprite;
        secondaryImage.color = new Color(1f, 1f, 1f, 0f);

        // Плавно увеличиваем прозрачность secondaryImage
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            secondaryImage.color = new Color(1f, 1f, 1f, alpha);
            uiImage.color = new Color(1f, 1f, 1f, 1f - alpha);
            yield return null;
        }

        // Устанавливаем конечные значения
        secondaryImage.color = new Color(1f, 1f, 1f, 1f);
        uiImage.color = new Color(1f, 1f, 1f, 0f);

        // Меняем спрайты местами
        uiImage.sprite = newSprite;
        uiImage.color = new Color(1f, 1f, 1f, 1f);
        secondaryImage.color = new Color(1f, 1f, 1f, 0f);

        isFading = false;
    }

    private bool IsColorMatch(Color a, Color b)
    {
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < colorThreshold;
    }
}