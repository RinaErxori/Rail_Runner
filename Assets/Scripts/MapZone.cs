using UnityEngine;

public class MapZone : MonoBehaviour
{
    public SpriteRenderer zoneRenderer;
    public Sprite highlightedSprite; // Картинка при наведении/клике
    public GameObject infoPanel;    // Дополнительная информация

    private Sprite defaultSprite;

    void Start()
    {
        defaultSprite = zoneRenderer.sprite;
    }

    void OnMouseEnter()
    {
        zoneRenderer.sprite = highlightedSprite;
    }

    void OnMouseExit()
    {
        zoneRenderer.sprite = defaultSprite;
    }

    void OnMouseDown()
    {
        if (infoPanel != null)
            infoPanel.SetActive(!infoPanel.activeSelf);
    }
}