using System;
using System.Collections.Generic;
using UnityEngine;

public class СreationMap : MonoBehaviour
{
    [NonSerialized] public GameObject[,] cellsMap;

    [SerializeField] public HashSet<GameObject> whiteListFade = new HashSet<GameObject>();

    [SerializeField] public HashSet<GameObject> blackListFade = new HashSet<GameObject>();

    [SerializeField] private GameObject prefabObj; // Префаб для ячеек
    [SerializeField] private GameObject allCell; // Родительский объект для всех ячеек
    [SerializeField] private GameObject blackCheckerPrefab; // Префаб для черных шашек
    [SerializeField] private GameObject whiteCheckerPrefab; // Префаб для белых шашек

    [SerializeField] private GameObject whiteCellPrefab; // Префаб для белых клеток
    [SerializeField] private GameObject blackCellPrefab; // Префаб для черных клеток
    [SerializeField] private GameObject backgroundParent;

    private int size = 8;

    [SerializeField] private float distancesX = 50f;
    [SerializeField] private float distancesY = 50f;

    [SerializeField] private Vector2 boardOrigin = new Vector2(-3.25f, -2.4f);

    private Transform cellTransform;

    private void Awake()
    {
        cellsMap = new GameObject[size, size];

        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                CreatMap(row, col);
                InstallationOfCheckers(row, col);
            }
        }
    }
    private void CreatMap(int row, int col)
    {
        Vector2 cellPosition = boardOrigin + new Vector2(col * distancesX, row * distancesY);
        Vector3 cellPositionVector3 = new Vector3(cellPosition.x, cellPosition.y, 0);

        GameObject cell = Instantiate(prefabObj, prefabObj.transform.localPosition + cellPositionVector3, Quaternion.identity);
        cell.name = ($"Cell_{row}_{col}");
        cellsMap[row, col] = cell;
        cell.transform.SetParent(allCell.transform);
        cellTransform = cell.transform;
        CreateCellBackground(row, col, cell.transform);
    }

    private void InstallationOfCheckers(int row, int col)
    {
        if ((row == 0 && (col == 0 || col == 2 || col == 4 || col == 6)) ||
            (row == 1 && (col == 1 || col == 3 || col == 5 || col == 7)) ||
            (row == 2 && (col == 0 || col == 2 || col == 4 || col == 6)))
        {
            GameObject whiteChecker = Instantiate(whiteCheckerPrefab, cellTransform.position, Quaternion.identity, cellTransform);
            whiteChecker.name = ($"whiteChecker_{row}_{col}");

            whiteListFade.Add(whiteChecker);
        }
        if ((row == 5 && (col == 1 || col == 3 || col == 5 || col == 7)) ||
            (row == 6 && (col == 0 || col == 2 || col == 4 || col == 6)) ||
            (row == 7 && (col == 1 || col == 3 || col == 5 || col == 7)))
        {
            GameObject blackChecker = Instantiate(blackCheckerPrefab, cellTransform.position, Quaternion.identity, cellTransform);
            blackChecker.name = ($"blackChecker_{row}_{col}");

            blackListFade.Add(blackChecker);
        }
    }
    private void CreateCellBackground(int row, int col, Transform cellTransform)
    {
        GameObject cellBackgroundPrefab = (row + col) % 2 == 0 ? whiteCellPrefab : blackCellPrefab;
        GameObject cellBackground = Instantiate(cellBackgroundPrefab, cellTransform.position, Quaternion.identity, cellTransform);

        cellBackground.name = $"Background_{row}_{col}";

        cellBackground.transform.SetParent(backgroundParent.transform);
    }
}
