using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SwitchUIElementsPlayer : MonoBehaviour
{
    [SerializeField] private СreationMap creationMap;
    [SerializeField] private CheckerDamage checkerDamage;

    [SerializeField] private TextMeshProUGUI blackText;
    [SerializeField] private TextMeshProUGUI blackTextInt;
    [SerializeField] private TextMeshProUGUI whiteText;
    [SerializeField] private TextMeshProUGUI whiteTextInt;
    [SerializeField] private CheckersMove checkersMove;
    private int previousCountWhite = 0;
    private int previousCountBlack = 0;

    [SerializeField] private HashSet<GameObject> whiteListFade = new HashSet<GameObject>();
    [SerializeField] public HashSet<GameObject> blackListFade = new HashSet<GameObject>();

    private void Awake()
    {
        whiteListFade.UnionWith(creationMap.whiteListFade);
        blackListFade.UnionWith(creationMap.blackListFade);
    }
    private void Start()
    {
        previousCountWhite = whiteListFade.Count;
        whiteTextInt.text = previousCountWhite.ToString();

        previousCountBlack = blackListFade.Count;
        blackTextInt.text = previousCountBlack.ToString();

    }
    private void Update()
    {
        if (checkersMove.value == 1)
        {
            if (whiteText != null)
            {
                whiteText.color = new Color(whiteText.color.r, whiteText.color.g, whiteText.color.b, 1f); // Меняем цвет белого текста
                whiteTextInt.color = new Color(whiteTextInt.color.r, whiteTextInt.color.g, whiteTextInt.color.b, 1f);
            }

            if (blackText != null)
            {
                blackText.color = new Color(blackText.color.r, blackText.color.g, blackText.color.b, 0.2f); // Меняем цвет черного текста
                blackTextInt.color = new Color(blackTextInt.color.r, blackTextInt.color.g, blackTextInt.color.b, 0.2f);
            }
        }
        else if (checkersMove.value == 2)// Добавляем обработку случая, когда значение не 1
        {
            if (whiteText != null)
            {
                whiteText.color = new Color(whiteText.color.r, whiteText.color.g, whiteText.color.b, 0.2f);
                whiteTextInt.color = new Color(whiteTextInt.color.r, whiteTextInt.color.g, whiteTextInt.color.b, 0.2f);
            }

            if (blackText != null)
            {
                blackText.color = new Color(blackText.color.r, blackText.color.g, blackText.color.b, 1f); // Возвращаем цвет, например, к белому
                blackTextInt.color = new Color(blackTextInt.color.r, blackTextInt.color.g, blackTextInt.color.b, 1f);
            }
        }

        whiteListFade = checkerDamage.GetWhiteList();
        blackListFade = checkerDamage.GetBlackList();

        if (whiteListFade.Count != previousCountWhite)
        {
            whiteTextInt.text = whiteListFade.Count.ToString();
            previousCountWhite = whiteListFade.Count;
        }
        else if (blackListFade.Count != previousCountBlack)
        {
            blackTextInt.text = blackListFade.Count.ToString();
            previousCountBlack = blackListFade.Count;
        }
    }
}
