using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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

    public GameObject gameTitle;

    readonly List<CardAnimationStruct> movements = new();

    public List<GameObject> characterCards = new();

    public float cardMovementSpeed = 5f; // Speed of movement
    public Sprite cardBackside;

    void Update()
    {
        for (int i = 0; i < movements.Count; i++)
        {
            MoveCard(movements[i].source, movements[i].target, movements[i].speed, out bool destinationReached);
            if (destinationReached)
            {
                if (movements[i].target.name == "TEMP")
                    Destroy(movements[i].target.gameObject);

                movements.RemoveAt(i);
                i--;
            }
        }
    }

    void MoveCard(Transform source, Transform destination, float speed, out bool reachedDestination)
    {
        // Calculate the direction to move towards the destination
        Vector3 direction = (destination.position - source.position).normalized;

        // Move towards the destination
        source.Translate(speed * Time.deltaTime * direction);

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
        Image classImage = runContent.Find("Class Image").GetComponent<Image>();
        string className = data.CurrentRun.ClassId.ToString().ToUpper();

        playerData.text = $"{className} - T{data.CurrentRun.CurrentFloor + 1}";
        classImage.sprite = Resources.Load<Sprite>($"Sprites/Characters/{className}/{className}");
    }

    public void StartChooseCharacterAnimation()
    {
        int numberOfClasses = Enum.GetValues(typeof(Classes)).Length;
        for (int i = 0; i < numberOfClasses - 1; i++)
        {
            GameObject characterCard = Instantiate(cardPrefab, outScreenPosition.position, outScreenPosition.rotation, characterParent);

            Classes classOfCard = (Classes)i;
            bool isClassUnlocked = IsClassUnlocked(classOfCard);

            LoadCharacterCard(characterCard, classOfCard.ToString(), isClassUnlocked);
            LoadScript(characterCard, classOfCard, isClassUnlocked);

            characterCards.Add(characterCard);

            Vector3 cardPosition = GetCardPositionOnTable(i);

            GameObject temp = CreateTempObject(cardPosition);
            float speed = 5 + ((10 - i) / 2);
            StartCardAnimation(characterCard.transform, temp.transform, speed);
        }
    }

    public bool IsClassUnlocked(Classes classOfCard)
    {
        return SaveManager.GetUnlockedClasses().Contains(classOfCard);
    }

    public void ChangeTitleVisibility(bool setActive)
    {
        gameTitle.SetActive(setActive);
    }

    GameObject CreateTempObject(Vector3 position)
    {
        GameObject temp = new("TEMP");
        temp.transform.parent = Canvas.transform;
        temp.transform.localPosition = position;

        return temp;
    }

    public void LoadScript(GameObject characterCard, Classes classOfCard, bool isUnlocked)
    {
        MenuCharacterCard cardScript = characterCard.GetComponent<MenuCharacterCard>();
        cardScript.LoadManager(MenuManager, classOfCard, isUnlocked);
    }

    public void LoadCharacterCard(GameObject card, string characterName, bool isUnlocked)
    {
        Image sprite = card.transform.Find("Icon").GetComponent<Image>();
        Image bg = card.transform.Find("Background").GetComponent<Image>();
        TextMeshProUGUI title = card.transform.Find("Title").GetComponent<TextMeshProUGUI>();

        sprite.sprite = Resources.Load<Sprite>($"Sprites/Characters/{characterName}/{characterName}");
        bg.color = IClass.GetCardBackgroundColor((Classes)Enum.Parse(typeof(Classes), characterName)); 

        if (isUnlocked)
        {
            title.text = characterName.ToUpper();
            return;
        }

        title.text = "LOCKED";
        sprite.color = Color.black;
    }

    public Vector3 GetCardPositionOnTable(int cardIndex)
    {
        if(cardIndex <= 2)
            return new Vector3(-350 + (350 * cardIndex), 250, 0);
        

        return new Vector3(-450 + (300 * (cardIndex - 3)), -200, 0);
    }

    public void StartMenuAnimation()
    {
        StartCardAnimation(cardLeft.transform, basePositionLeft, 5f);
        StartCardAnimation(cardFront.transform, basePositionCenter, 5f);
        StartCardAnimation(cardRight.transform, basePositionRight, 5f);
    }

    public void StartMenuClearAnimation()
    {
        StartCardAnimation(cardLeft.transform, outScreenPosition, 5f);
        StartCardAnimation(cardFront.transform, outScreenPosition, 5f);
        StartCardAnimation(cardRight.transform, outScreenPosition, 5f);
    }

    public void StartCardAnimation(Transform source, Transform target, float speed)
    {
        movements.Add(new(source, target, speed));
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

    public struct CardAnimationStruct
    {
        public Transform source;
        public Transform target;
        public float speed;

        public CardAnimationStruct(Transform source, Transform target, float speed)
        {
            this.source = source; 
            this.target = target; 
            this.speed = speed;
        }
    }
}