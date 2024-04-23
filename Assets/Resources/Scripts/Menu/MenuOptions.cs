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
        source.Translate(direction * cardMovementSpeed * Time.deltaTime);

        // Check if the object reached the destination
        reachedDestination = Vector3.Distance(source.position, destination.position) < 0.01f;
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

    public enum CardPosition
    {
        Left,
        Center,
        Right
    }
}
