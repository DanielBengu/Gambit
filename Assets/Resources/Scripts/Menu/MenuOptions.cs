using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CardsManager;
using static MenuOptions;

public class MenuOptions : MonoBehaviour
{
    public GameObject Canvas;

    public MenuManager MenuManager;

    public GameObject cardFront;
    public GameObject cardLeft;
    public GameObject cardRight;

    public Transform basePositionLeft;
    public Transform basePositionRight;
    public Transform basePositionCenter;

    public Transform outScreenPosition;

    public Transform characterParent;

    public GameObject cardPrefab;

    readonly List<Tuple<Transform, Transform>> movements = new();

    public List<GameObject> characterCards = new();

    public float cardMovementSpeed = 5f; // Speed of movement
    public Sprite cardBackside;

    void Update()
    {
        for (int i = 0; i < movements.Count; i++)
        {
            MoveCard(movements[i].Item1, movements[i].Item2, out bool destinationReached);
            if (destinationReached)
            {
                if (movements[i].Item2.name == "TEMP")
                    Destroy(movements[i].Item2.gameObject);

                movements.RemoveAt(i);
                i--;
            }
        }
    }

    void MoveCard(Transform source, Transform destination, out bool reachedDestination)
    {
        // Calculate the direction to move towards the destination
        Vector3 direction = (destination.position - source.position).normalized;

        // Move towards the destination
        source.Translate(cardMovementSpeed * Time.deltaTime * direction);

        // Check if the object reached the destination
        reachedDestination = Vector3.Distance(source.position, destination.position) < 0.1f;
    }

    public void LoadRunInfo(PlayerData data)
    {
       LoadLeftCard(data);
    }

    void LoadLeftCard(PlayerData data)
    {
        Transform runContent = cardLeft.transform.GetChild(0);
        TextMeshProUGUI playerData = runContent.Find("RunInfo").GetComponent<TextMeshProUGUI>();
        string className = data.CurrentRun.ClassId.ToString();
        playerData.text = $"{className} - {data.CurrentRun.CurrentFloor}F";
    }

    public void StartChooseCharacterAnimation()
    {
        int numberOfClasses = Enum.GetValues(typeof(Classes)).Length;

        for (int i = 0; i < numberOfClasses - 1; i++)
        {
            GameObject characterCard = Instantiate(cardPrefab, outScreenPosition.position, outScreenPosition.rotation, characterParent);

            Classes classOfCard = (Classes)i;
            LoadCharacterCard(characterCard, classOfCard.ToString());
            LoadScript(characterCard, classOfCard);

            characterCards.Add(characterCard);

            Vector3 cardPosition = GetCardPositionOnTable(i);

            GameObject temp = CreateTempObject(cardPosition); 
            StartCardAnimation(characterCard.transform, temp.transform);
        }
    }

    GameObject CreateTempObject(Vector3 position)
    {
        GameObject temp = new("TEMP");
        temp.transform.parent = Canvas.transform;
        temp.transform.localPosition = position;

        return temp;
    }

    public void LoadScript(GameObject characterCard, Classes classOfCard)
    {
        MenuCharacterCard cardScript = characterCard.GetComponent<MenuCharacterCard>();
        cardScript.LoadManager(MenuManager, classOfCard);
    }

    public void LoadCharacterCard(GameObject card, string characterName)
    {
        TextMeshProUGUI title = card.transform.Find("Title").GetComponent<TextMeshProUGUI>();
        Image sprite = card.transform.Find("Sprite").GetComponent<Image>();

        title.text = characterName;
        sprite.sprite = Resources.Load<Sprite>($"Sprites/Characters/{characterName}/{characterName}");
    }

    public Vector3 GetCardPositionOnTable(int cardIndex)
    {
        if(cardIndex <= 2)
            return new Vector3(-500 + (500 * cardIndex), 250, 0);
        

        return new Vector3(-750 + (500 * (cardIndex - 3)), -250, 0);
    }

    public void StartMenuAnimation()
    {
        StartCardAnimation(cardLeft.transform, basePositionLeft);
        StartCardAnimation(cardFront.transform, basePositionCenter);
        StartCardAnimation(cardRight.transform, basePositionRight);
    }

    public void StartMenuClearAnimation()
    {
        StartCardAnimation(cardLeft.transform, outScreenPosition);
        StartCardAnimation(cardFront.transform, outScreenPosition);
        StartCardAnimation(cardRight.transform, outScreenPosition);
    }

    public void StartCardAnimation(Transform source, Transform target)
    {
        movements.Add(new(source, target));
    }

    public void CoverCard(CardPosition card)
    {
        GameObject cardToCover = cardLeft; 

        switch (card)
        {
            case CardPosition.Left:
                cardToCover = cardLeft;
                break;
            case CardPosition.Center:
                cardToCover = cardFront;
                break;
            case CardPosition.Right:
                cardToCover = cardRight;
                break;
        }

        cardToCover.GetComponent<Image>().sprite = cardBackside;
        cardToCover.transform.GetChild(0).gameObject.SetActive(false);
    }

    public enum CardPosition
    {
        Left,
        Center,
        Right
    }
}
