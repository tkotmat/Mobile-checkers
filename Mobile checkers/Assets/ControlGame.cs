using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGame : BasicOptionControls
{
    private int captureCount = 0;
    protected override GameObject[,] cellsMap { get; set; }

    [SerializeField] private GameObject pointObject;
    [SerializeField] private ÑreationMap creationMap;
    [SerializeField] private CheckersMove checkersMove;

    private Dictionary<GameObject, bool> objectVisibility = new Dictionary<GameObject, bool>();
    private List<GameObject> objectDestoy = new List<GameObject>();

    private GameObject lastRightObject;
    private GameObject lastLeftObject;
    private GameObject previousObject;
    private GameObject pointMoveSetCheckersMove;

    private Vector2Int rightPosition;
    private Vector2Int leftPosition;
    private Vector2Int lastPosition;

    private bool isRecapture = false;
    private bool isMoving = true;


    private List<Vector2Int> singleDirections = new List<Vector2Int>()
    {
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)
    };

    private void Awake()
    {
        cellsMap = creationMap.cellsMap;

        whiteListFade.UnionWith(creationMap.whiteListFade);
        blackListFade.UnionWith(creationMap.blackListFade);

    }

    private void Start()
    {
        for (int x = 0; x < cellsMap.GetLength(0); x++)
        {
            for (int y = 0; y < cellsMap.GetLength(1); y++)
            {
                GameObject cellObject = cellsMap[x, y];
                objectVisibility[cellObject] = false;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = HandleMouseInput();

            if (hit.collider != null && checkersMove.isMovingTrue() == true)
            {
                HandleHit(hit.collider.gameObject);

                HandlerÑaptureProcess(hit.collider.gameObject);
            }
            
        }
        RemoveUnfadedObjects();
    }

    private void HandleHit(GameObject hitObject)
    {
        if (whiteListFade.Contains(hitObject) && checkersMove.value == 1 ||
            blackListFade.Contains(hitObject) && checkersMove.value == 2)
        {
            objectDestoy.Clear();
            bool mandatoryFight = false;

            GameObject parentObject = hitObject.transform.parent.gameObject;

            if (previousObject != null && previousObject != parentObject)
            {
                SetObjectTransparency(previousObject, 0f);
                objectVisibility[previousObject] = false;
            }

            if (objectVisibility[parentObject] == false)
            {
                SetObjectTransparency(parentObject, 1.0f);
                objectVisibility[parentObject] = true;

                Vector2Int position = FindObjectIntCellsMapPosition(parentObject);
                if (position.x != -1 && position.y != -1)
                {
                    if (lastRightObject != null)
                    {
                        Destroy(lastRightObject);
                    }
                    if (lastLeftObject != null)
                    {
                        Destroy(lastLeftObject);
                    }

                    if (whiteListFade.Contains(hitObject) && checkersMove.value == 1)
                    {
                        rightPosition = new Vector2Int(position.x + 1, position.y + 1);
                        leftPosition = new Vector2Int(position.x + 1, position.y - 1);
                    }
                    else if (blackListFade.Contains(hitObject) && checkersMove.value == 2)
                    {
                        rightPosition = new Vector2Int(position.x - 1, position.y - 1);
                        leftPosition = new Vector2Int(position.x - 1, position.y + 1);
                    }

                    ProcessPosition(rightPosition, blackListFade, new Vector2Int(1, 1), "Right", ref isRecapture, ref mandatoryFight, objectDestoy);
                    ProcessPosition(leftPosition, blackListFade, new Vector2Int(1, -1), "Left", ref isRecapture, ref mandatoryFight, objectDestoy);

                    ProcessPosition(rightPosition, whiteListFade, new Vector2Int(-1, -1), "Right", ref isRecapture, ref mandatoryFight, objectDestoy);
                    ProcessPosition(leftPosition, whiteListFade, new Vector2Int(-1, 1), "Left", ref isRecapture, ref mandatoryFight, objectDestoy);

                    if (mandatoryFight == false)
                    {
                        if (ProcessingArrayBoundaries(rightPosition)) createPointObject(rightPosition, pointObject, "point", "Right");
                        if (ProcessingArrayBoundaries(leftPosition)) createPointObject(leftPosition, pointObject, "point", "Left");
                    }
                }
            }
            else
            {
                SetObjectTransparency(parentObject, 0f);
                objectVisibility[parentObject] = false;
            }
            previousObject = parentObject;
        }
    }
    private void HandlerÑaptureProcess(GameObject pointHit)
    {
        string hitTag = pointHit.tag;
        string hitName = pointHit.name;

        if ((hitTag == "Point" && isRecapture) || hitName == "pointDamageRight" || hitName == "pointDamageLeft")
        {
            GameObject parentPointHit = pointHit.transform.parent.gameObject;
            Vector2Int indexPositionsOnMapParentPointHit = FindObjectIntCellsMapPosition(parentPointHit);

            if (indexPositionsOnMapParentPointHit.x != -1 && indexPositionsOnMapParentPointHit.y != -1)
            {
                bool successfulCapture = false;
                int currentCheckerValue = checkersMove.value;
                var currentListFade = currentCheckerValue == 1 ? blackListFade : whiteListFade;

                foreach (Vector2Int singleDirection in singleDirections)
                {
                    Vector2Int newPosition = indexPositionsOnMapParentPointHit + singleDirection;
                    if (ProcessingArrayBoundaries(newPosition))
                    {
                        GameObject checker = cellsMap[newPosition.x, newPosition.y].transform.childCount > 0
                            ? cellsMap[newPosition.x, newPosition.y].transform.GetChild(0).gameObject
                            : null;

                        if (checker != null && currentListFade.Contains(checker))
                        {
                            newPosition += singleDirection;

                            if (newPosition != lastPosition && ProcessingArrayBoundaries(newPosition) && cellsMap[newPosition.x, newPosition.y].transform.childCount == 0)
                            {
                                objectDestoy.Add(checker);
                                captureCount++;
                                StartCoroutine(CreateCapturePoint(newPosition));
                                successfulCapture = true;
                            }
                        }
                    }
                }

                if (successfulCapture)
                {
                    lastPosition = indexPositionsOnMapParentPointHit;
                    isMoving = true;
                }
                else isMoving = false;

                isRecapture = false;
                pointMoveSetCheckersMove = pointHit;
                StartCoroutine(DestroyNextPointWithDelay(pointHit, 0.2f));
            }
        }
        else if (hitTag == "Point")
        {
            GameObject point = pointHit;
            pointMoveSetCheckersMove = point;
            StartCoroutine(DestroyNextPointWithDelay(point, 0.1f));
        }
    }

    private IEnumerator DestroyNextPointWithDelay(GameObject point, float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(point);

        yield return null;
        GameObject pointLast = GameObject.FindWithTag("Point");

        if (pointLast != null)
        {
            Destroy(pointLast);
        }
    }

    IEnumerator CreateCapturePoint(Vector2Int position)
    {
        yield return new WaitForSeconds(0.3f);
        createPointObject(position, pointObject, "pointDamage", "Right");
    }

    public void ProcessPosition(Vector2Int startPos, HashSet<GameObject> fadeList, Vector2Int direction, string name, ref bool isRecapture, ref bool mandatoryFight,List<GameObject> objectDestoy)
    {

        if (ProcessingArrayBoundaries(startPos) && startPos.x + direction.x < cellsMap.GetLength(0) && startPos.y + direction.y < cellsMap.GetLength(1) &&
            cellsMap[startPos.x, startPos.y].transform.childCount > 0)
        {
            GameObject checker = cellsMap[startPos.x, startPos.y].transform.GetChild(0).gameObject;

            if (fadeList.Contains(checker))
            {
                Vector2Int targetPos = new Vector2Int(startPos.x + direction.x, startPos.y + direction.y);

                if (ProcessingArrayBoundaries(targetPos) && cellsMap[targetPos.x, targetPos.y].transform.childCount == 0)
                {
                    objectDestoy.Add(checker);
                    mandatoryFight = true;
                    isRecapture = true;
                    createPointObject(targetPos, pointObject, "pointDamage", name);
                }
            }
        }
    }

    protected override void SetObjectTransparency(GameObject obj, float alpha)
    {
        Color color = obj.GetComponent<SpriteRenderer>().color;
        color.a = alpha;
        obj.GetComponent<SpriteRenderer>().color = color;
        objectVisibility[obj] = alpha > 0;
    }

    private void createPointObject(Vector2Int position, GameObject point, string name, string rotalil)
    {
        GameObject parentCell = cellsMap[position.x, position.y];
        if (parentCell != null && !HasNonPointChildren(parentCell, "Point"))
        {
            if (rotalil == "Right")
            {
                lastRightObject = Instantiate(point, parentCell.transform.position, Quaternion.identity);
                lastRightObject.transform.parent = parentCell.transform;
                lastRightObject.name = $"{name}Right";
            }
            else if (rotalil == "Left")
            {
                lastLeftObject = Instantiate(point, parentCell.transform.position, Quaternion.identity);
                lastLeftObject.transform.parent = parentCell.transform;
                lastLeftObject.name = $"{name}Left";
            }
        }
    }
    protected void RemoveUnfadedObjects()
    {
        bool hasFadedObject = false;
        foreach (var state in objectVisibility.Values)
        {
            if (state)
            {
                hasFadedObject = true;
                break;
            }
        }

        if (hasFadedObject == false)
        {
            if (lastRightObject != null)
            {
                Destroy(lastRightObject);
                lastRightObject = null;
            }

            if (lastLeftObject != null)
            {
                Destroy(lastLeftObject);
                lastLeftObject = null;
            }
        }
    }
    public GameObject FadedObject()
    {
        GameObject fadedObject = null;
        bool hasFadedObject = false;

        foreach (var entry in objectVisibility)
        {
            if (entry.Value)
            {
                hasFadedObject = true;
                fadedObject = entry.Key;
                break;
            }
        }
        if (hasFadedObject && fadedObject != null)
        {
            if (fadedObject.transform.childCount > 0)
            {
                Transform childTransform = fadedObject.transform.GetChild(0);
                GameObject childObject = childTransform.gameObject;

                return childObject;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    public List<GameObject> GetobjectDestroy() => objectDestoy;
    public bool GetIsRecapture() => isRecapture;
    public GameObject GetIsPointMove() => pointMoveSetCheckersMove;
    public int MultiThreadedMove() => captureCount;
    public bool GetIsMovingNext() => isMoving;
}