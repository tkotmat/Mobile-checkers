using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckersBot : BasicOptionControls
{
    [SerializeField] private ControlGame game;
    [SerializeField] private CheckersMove move;
    [SerializeField] private �reationMap creationMap; // �������� "�reationMap" �� "CreationMap"
    [SerializeField] private CheckerDamage checkerDamage;

    // ��������� ������ ��� �������� �������� �����
    protected override GameObject[,] cellsMap { get; set; }

    // ����� ������ ��������, ��� ������� ����� ��������� �������� ��� ��������
    [SerializeField] public HashSet<GameObject> whiteListFade1 = new HashSet<GameObject>();

    // ������ ������ ��������, ��� ������� ����� ��������� �������� ��� ��������
    [SerializeField] public HashSet<GameObject> blackListFade1 = new HashSet<GameObject>();

    private List<GameObject> fadeObj = new List<GameObject>();
    private List<Vector2Int> fadeObjPosition = new List<Vector2Int>();
    private bool needsUpdate = true;

    Vector2Int position = new Vector2Int(-1, -1); // ������������� � -1, -1 ��� ������������

    private void Awake()
    {
        cellsMap = creationMap.cellsMap;

        if (checkerDamage != null) // �������� �� null
        {
            whiteListFade1 = checkerDamage.GetWhiteList();
            blackListFade1 = checkerDamage.GetBlackList();
        }
        else
        {
            Debug.LogWarning("CheckerDamage �� ���������� � CheckersBot.");
        }
    }
    private void Update()
    {
        if (move.value == 2)
        {
            needsUpdate = false; // ����� �����, ����� �������� �� ����������� ��������
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

            // ���� ������� ������ ���������� ���������, ��������� ����
            if (fadeObj.Count >= blackListFade1.Count)
            {
                yield break; // ���������� ��������
            }
            // �������� � 1 ������� ����� ����������
            yield return new WaitForSeconds(1f);
        }
    }
}
