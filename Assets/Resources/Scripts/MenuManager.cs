using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Sprite cardBackside;

    public GameObject cardLeft;
    public GameObject cardRight;

    void Start()
    {
        if (true)
        {
            CoverCard(cardLeft);
            CoverCard(cardRight);
        }
    }

    void CoverCard(GameObject card)
    {
        card.GetComponent<Image>().sprite = cardBackside;
        card.transform.GetChild(0).gameObject.SetActive(false);
    }
}
