using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCardInstance : MonoBehaviour
{
    ActionCard actionCard;
    FightManager fightManager;
    bool isDragging = false;
    Vector3 originalPosition;
    Vector3 originalScale;
    int originalSiblingIndex;

    static readonly float BOUNDARY_THRESHOLD = 2f;
    static readonly float CARD_SIZEUP = 1.5f;

    public void LoadActionCard(ActionCard actionCard, FightManager manager)
    {
        this.actionCard = actionCard;
        fightManager = manager;
        originalPosition = transform.position; // Store the original position
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (InputManager.IsClickDown() && !fightManager.IsGameOnStandby())
            StartDraggingCard();

        if (isDragging)
            MoveCard();

        if (InputManager.IsClickUp() || (isDragging && fightManager.IsGameOnStandby()))
            ReleaseCard();
    }

    void StartDraggingCard()
    {
        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            isDragging = true;
            transform.localScale = originalScale * CARD_SIZEUP; // Scale up the card
            originalSiblingIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
        }
    }

    void MoveCard()
    {
        Vector2 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    void ReleaseCard()
    {
        if (!isDragging)
            return;

        // Check if released within boundaries
        Vector2 releasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(originalPosition, releasePosition) > BOUNDARY_THRESHOLD)
        {
            PlayCard();
        }
        else
        {
            CancelCard();
        }

        isDragging = false;
    }

    void PlayCard()
    {
        fightManager.PlayActionCard(actionCard, FightManager.Character.Player);

        if (fightManager.Player.status != FightManager.CharacterStatus.Playing)
            fightManager.PlayEnemyTurn();
            
        Destroy(gameObject);
    }

    void CancelCard()
    {
        // Release the card back to its original size
        transform.localScale = originalScale;
        transform.position = originalPosition; // Move the card back to its original position
        transform.SetSiblingIndex(originalSiblingIndex);
    }
}