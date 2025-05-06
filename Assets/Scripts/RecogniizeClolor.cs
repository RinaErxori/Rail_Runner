using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecognizeColor : MonoBehaviour, IPointerClickHandler
{
    private Image uiImage;
    private Texture2D targetTexture;
    private RectTransform rectTransform;

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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Получаем локальные координаты клика относительно UI-элемента
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
                if (pixelColor.a < 0.1f)
                {
                    Debug.Log("Прозрачный пиксель!");
                }
                else
                {
                    Debug.Log($"Цвет пикселя: {pixelColor} (R: {pixelColor.r}, G: {pixelColor.g}, B: {pixelColor.b})");
                }
            }
            else
            {
                Debug.LogWarning("Клик вне текстуры!");
            }
        }
    }
}