using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckersMove : BasicOptionControls
{
    protected override GameObject[,] cellsMap { get; set; }

    [SerializeField] private ControlGame controlGame;
    [SerializeField] private float speedCheckers = 5.0f;
    [NonSerialized] public int value = 1;
    private int currentTargetIndex = 0;

    private List<int> numbersCell = new List<int>();
    private List<Vector2> pointList = new List<Vector2>();

    private GameObject fadedObject;
    private GameObject parantPoint;

    private Vector2 targetPosition;
    private Vector2Int parentVector;

    private bool isMoving = false;
    private bool moveObjectTrue = true;
    private bool changePlayer = false;
    private bool isMovingRepeat = false;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = HandleMouseInput();
            fadedObject = controlGame.FadedObject();


            if (hit.collider != null && controlGame.GetIsPointMove() == hit.collider.gameObject)
            {
                if (hit.collider.tag == "Point" && hit.collider.name == "pointLeft" || hit.collider.name == "pointRight" && isMoving == false)
                {
                    ProcessPoint(controlGame.GetIsPointMove());
                }
                else if (hit.collider.tag == "Point" && controlGame.GetIsRecapture() == true || hit.collider.name == "pointDamageLeft" || hit.collider.name == "pointDamageRight")
                {
                    if (controlGame.MultiThreadedMove() <= 0 && isMoving == false)
                    {
                        ProcessPoint(controlGame.GetIsPointMove());
                    }
                    else if (controlGame.MultiThreadedMove() > 0 && isMovingRepeat == false)
                    {
                        ProcessPointTwo(controlGame.GetIsPointMove());
                    }
                }
            }
        }

        if (isMoving == true && fadedObject != null && targetPosition != null)
        {
            moveObjectTrue = false;
            Vector2 moveTowards = Vector2.MoveTowards(fadedObject.transform.position, targetPosition, speedCheckers * Time.deltaTime);
            fadedObject.transform.position = moveTowards;

            if (Vector2.Distance(fadedObject.transform.position, targetPosition) < 0.1f)
            {
                fadedObject.transform.position = targetPosition;

                fadedObject.transform.SetParent(parantPoint.transform);

                numbersCell = Text(parantPoint);
                if (fadedObject.name.StartsWith("whiteChecker"))
                {
                    fadedObject.name = ($"whiteChecker_{numbersCell[0]}_{numbersCell[1]}");
                }
                else
                {
                    fadedObject.name = ($"blackChecker_{numbersCell[0]}_{numbersCell[1]}");
                }

                if (changePlayer == false)
                {
                    value += 1;
                }
                else
                {
                    value -= 1;
                }
                changePlayer = !changePlayer;

                moveObjectTrue = true;
                isMoving = false;
            }
        }
        else if (isMovingRepeat == true && fadedObject != null && pointList.Count > 0 && controlGame.GetIsMovingNext() == false)
        {
            moveObjectTrue = false;
            if (currentTargetIndex < pointList.Count)
            {
                Vector2 targetPosition = pointList[currentTargetIndex];
                Vector2 moveTowards = Vector2.MoveTowards(fadedObject.transform.position, targetPosition, speedCheckers * Time.deltaTime);
                fadedObject.transform.position = moveTowards;

                if (Vector2.Distance(fadedObject.transform.position, targetPosition) < 0.1f)
                {
                    fadedObject.transform.position = targetPosition;
                    fadedObject.transform.SetParent(parantPoint.transform);

                    numbersCell = Text(parantPoint);
                    fadedObject.name = fadedObject.name.StartsWith("whiteChecker") ? $"whiteChecker_{numbersCell[0]}_{numbersCell[1]}" : $"blackChecker_{numbersCell[0]}_{numbersCell[1]}";

                    currentTargetIndex++;
                }
            }
            else
            {
                if (changePlayer == false)
                {
                    value += 1;
                }
                else
                {
                    value -= 1;
                }
                changePlayer = !changePlayer;

                moveObjectTrue = true;
                isMovingRepeat = false;
                currentTargetIndex = 0;
                pointList.Clear();
            }
        }
    }
    private void ProcessPoint(GameObject point)
    {
        isMoving = true;
        parantPoint = point.transform.parent.gameObject;

        GameObject parentCell = fadedObject?.transform.parent?.gameObject;
        if (parentCell != null)
        {
            SetObjectTransparency(parentCell, 0f);
        }
        targetPosition = point.transform.position;
    }
    private void ProcessPointTwo(GameObject pointNext)
    {
        parantPoint = pointNext.transform.parent.gameObject;
        if (pointNext != null)
        {
            pointList.Add(pointNext.transform.position);
        }

        GameObject parentCell = fadedObject?.transform.parent?.gameObject;
        if (parentCell != null && controlGame.GetIsMovingNext() == false)
        {
            SetObjectTransparency(parentCell, 0f);
        }

        if (controlGame.GetIsMovingNext() == false)
        {
            isMovingRepeat = true;
        }
    }

    public bool isMovingTrue()
    {
        if (moveObjectTrue == true)
        {
            return true;
        }
        return false;

    }
    private List<int> Text(GameObject parament)
    {
        List<int> numbersList = new List<int>();
        string lastname = parament.name;

        char[] chars = lastname.ToCharArray();

        foreach (char ch in chars)
        {
            if (char.IsDigit(ch))
            {
                int numbers = int.Parse(ch.ToString());
                numbersList.Add(numbers);
            }
        }
        return numbersList;
    }
}
