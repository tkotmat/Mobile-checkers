using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckersBot : BasicOptionControls
{
    [SerializeField] private ControlGame game;
    [SerializeField] private CheckersMove move;
    [SerializeField] private СreationMap creationMap; // Исправил "СreationMap" на "CreationMap"
    [SerializeField] private CheckerDamage checkerDamage;

    // Двумерный массив для хранения объектов карты
    protected override GameObject[,] cellsMap { get; set; }

    // Белый список объектов, для которых будет применена анимация или действие
    [SerializeField] public HashSet<GameObject> whiteListFade1 = new HashSet<GameObject>();

    // Черный список объектов, для которых будет применена анимация или действие
    [SerializeField] public HashSet<GameObject> blackListFade1 = new HashSet<GameObject>();

    private List<GameObject> fadeObj = new List<GameObject>();
    private List<Vector2Int> fadeObjPosition = new List<Vector2Int>();
    private bool needsUpdate = true;

    Vector2Int position = new Vector2Int(-1, -1); // Инициализация с -1, -1 для безопасности

    private void Awake()
    {
        cellsMap = creationMap.cellsMap;

        if (checkerDamage != null) // Проверка на null
        {
            whiteListFade1 = checkerDamage.GetWhiteList();
            blackListFade1 = checkerDamage.GetBlackList();
        }
        else
        {
            Debug.LogWarning("CheckerDamage не установлен в CheckersBot.");
        }
    }
    private void Update()
    {
        if (move.value == 2)
        {
            needsUpdate = false; // Сброс флага, чтобы корутина не запускалась повторно
            StartCoroutine(ProcessFadesWithDelay());
        }
    }

    private IEnumerator ProcessFadesWithDelay()
    {
        foreach (GameObject fade in blackListFade1)
        {
            GameObject parentFade = fade.transform.parent?.gameObject;
            if (parentFade != null)
            {
                position = FindObjectIntCellsMapPosition(parentFade);

                if (position.x != -1 && position.y != -1)
                {
                    if (position.x >= 0 && position.x < cellsMap.GetLength(0) &&
                        position.y >= 0 && position.y < cellsMap.GetLength(1))
                    {
                        if (position.x - 1 >= 0 && position.y - 1 >= 0 &&
                            position.x - 1 < cellsMap.GetLength(0) &&
                            position.y - 1 < cellsMap.GetLength(1))
                        {
                            if (cellsMap[position.x - 1, position.y - 1].transform.childCount == 0)
                            {
                                fadeObj.Add(fade);
                                fadeObjPosition.Add(position);

                                Debug.Log(fade);
                            }
                        }
                    }
                }
            }

            // Если найдено нужное количество элементов, завершаем цикл
            if (fadeObj.Count >= blackListFade1.Count)
            {
                yield break; // Завершение корутины
            }
            // Задержка в 1 секунду между проверками
            yield return new WaitForSeconds(1f);
        }
    }
}
