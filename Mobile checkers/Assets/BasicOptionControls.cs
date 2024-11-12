using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BasicOptionControls : MonoBehaviour
{
    protected abstract GameObject[,] cellsMap { get; set; }

    [SerializeField] public HashSet<GameObject> whiteListFade = new HashSet<GameObject>();
    [SerializeField] public HashSet<GameObject> blackListFade = new HashSet<GameObject>();

    protected RaycastHit2D HandleMouseInput()
    {
        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    }
    protected Vector2Int FindObjectIntCellsMapPosition(GameObject targetObject)
    {
        for (int i = 0; i < cellsMap.GetLength(0); i++)
        {
            for (int j = 0; j < cellsMap.GetLength(1); j++)
            {
                if (cellsMap[i, j] == targetObject)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    protected bool ProcessingArrayBoundaries(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < cellsMap.GetLength(0) && pos.y >= 0 && pos.y < cellsMap.GetLength(1);
    }
    protected bool HasNonPointChildren(GameObject parent, string nameParentObject)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.tag != nameParentObject)
            {
                return true;
            }
        }
        return false;
    }
    protected virtual void SetObjectTransparency(GameObject obj, float alpha)
    {
        Color color = obj.GetComponent<SpriteRenderer>().color;
        color.a = alpha;
        obj.GetComponent<SpriteRenderer>().color = color;
    }
}
