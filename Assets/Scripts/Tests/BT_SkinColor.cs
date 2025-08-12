using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BT_SkinColor : MonoBehaviour, IPointerDownHandler
{
    Image image;
    Color color;
    void Awake()
    {
        image = GetComponent<Image>();
    }
    void FixedUpdate()
    {
        color = image.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        BodyTestUI.SetColor(color);
    }
}