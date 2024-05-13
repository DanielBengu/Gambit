using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCardInstance : MonoBehaviour
{
    ActionCard actionCard;
    FightManager fightManager;

    public void LoadActionCard(ActionCard actionCard, FightManager manager)
    {
        this.actionCard = actionCard;
        fightManager = manager;
    }

    void Update()
    {
        if (!InputManager.IsClick()) // Left mouse button
            return;

        Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            fightManager.PlayActionCard(actionCard, FightManager.Character.Player);
            Destroy(gameObject);
        }
    }
}
