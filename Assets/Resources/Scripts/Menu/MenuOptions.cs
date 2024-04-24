using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public GameObject cardFront;
    public GameObject cardLeft;
    public GameObject cardRight;

    public Transform basePositionLeft;
    public Transform basePositionRight;
    public Transform basePositionCenter;

    private bool isMovingLeft = false;
    private bool isMovingCenter = false;
    private bool isMovingRight = false;

    public float cardMovementSpeed = 5f; // Speed of movement
    public Sprite cardBackside;

    void Update()
    {
        if (isMovingLeft)
            MoveCardTowardsBasePosition(CardPosition.Left);
        if (isMovingCenter)
            MoveCardTowardsBasePosition(CardPosition.Center);
        if (isMovingRight)
            MoveCardTowardsBasePosition(CardPosition.Right);
    }

    void MoveCardTowardsBasePosition(CardPosition card)
    {
        switch (card)
        {
            case CardPosition.Left:
                MoveCard(cardLeft.transform, basePositionLeft, out bool reachedDestinationLeft);
                if (reachedDestinationLeft)
                    isMovingLeft = false;
                break;
            case CardPosition.Center:
                MoveCard(cardFront.transform, basePositionCenter, out bool reachedDestinationCenter);
                if (reachedDestinationCenter)
                    isMovingCenter = false;
                break;
            case CardPosition.Right:
                MoveCard(cardRight.transform, basePositionRight, out bool reachedDestinationRight);
                if (reachedDestinationRight)
                    isMovingRight = false;
                break;
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
        string className = GetClassNameFromId(data.CurrentRun.ClassId);
        playerData.text = $"{className} - {data.CurrentRun.CurrentFloor}F";
    }
    public void StartCardAnimation()
    {
        isMovingLeft = true;
        isMovingCenter = true;
        isMovingRight = true;
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

    string GetClassNameFromId(int id)
    {
        ClassData classes = JSONManager.GetFileFromJSON<ClassData>(JSONManager.CLASSES_PATH);
        return classes.Classes[id].Name;
    }

    public enum CardPosition
    {
        Left,
        Center,
        Right
    }
}
