using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;  // ������ � TextMeshPro
    [SerializeField] private Vector3 offset;      // ����� ������ ������������ �������

    void Start()
    {
        // ��������, ��� Raycast �������� � TextMeshPro
        TextMeshProUGUI textMeshPro = tooltip.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.raycastTarget = false; // ��������� ��������� �����
        }
    }

    void Update()
    {
        if (tooltip.activeSelf)
        {
            // ������ �� ���������� ����
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
