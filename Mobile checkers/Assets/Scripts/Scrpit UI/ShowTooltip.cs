using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;  // Объект с TextMeshPro
    [SerializeField] private Vector3 offset;      // Сдвиг текста относительно курсора

    void Start()
    {
        // Убедимся, что Raycast отключен у TextMeshPro
        TextMeshProUGUI textMeshPro = tooltip.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.raycastTarget = false; // Отключаем обработку лучей
        }
    }

    void Update()
    {
        if (tooltip.activeSelf)
        {
            // Следим за положением мыши
            tooltip.transform.position = Input.mousePosition + offset;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}
