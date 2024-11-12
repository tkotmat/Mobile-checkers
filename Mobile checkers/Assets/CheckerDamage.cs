using System.Collections.Generic;
using UnityEngine;

public class CheckerDamage : BasicOptionControls
{
    protected override GameObject[,] cellsMap { get; set; }

    [SerializeField] private ÑreationMap creationMap;
    [SerializeField] private ControlGame controlGame;
    [SerializeField] private CheckersMove checkersMove;

    private GameObject fadedObject;

    private void Awake()
    {
        cellsMap = creationMap.cellsMap;

        whiteListFade.UnionWith(creationMap.whiteListFade);
        blackListFade.UnionWith(creationMap.blackListFade);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = HandleMouseInput();

            if (hit.collider != null)
            {
                if (controlGame.GetIsMovingNext() == false && hit.collider.gameObject.name == "pointDamageRight" || hit.collider.gameObject.name == "pointDamageLeft")
                {
                    List<GameObject> royRight = controlGame.GetobjectDestroy();

                    if (royRight != null)
                    {
                        float delay = 0.5f;
                        foreach (var objectDestroyRight in royRight)
                        {
                            Destroy(objectDestroyRight, delay);

                            if (whiteListFade.Contains(objectDestroyRight))
                            {
                                whiteListFade.Remove(objectDestroyRight);
                            }
                            else if (blackListFade.Contains(objectDestroyRight))
                            {
                                blackListFade.Remove(objectDestroyRight);
                            }
                            delay += 0.2f;
                        }
                    }
                }
            }
        }
    }
    public HashSet<GameObject> GetWhiteList()
    {
        return whiteListFade;
    }
    public HashSet<GameObject> GetBlackList()
    {
        return blackListFade;
    }
}
