using Assets.Resources.Scripts.Fight;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EffectsManager.MovingObject;
using static EnemyManager;
using static FightManager;

public class GameUIManager : MonoBehaviour
{
    #region Player UI
    public Slider playerSlider;
    public Image playerSliderColor;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI playerMaxScoreText;
    public TextMeshProUGUI playerTitleText;
    public TextMeshProUGUI playerHPText;
    public Slider playerHPBar;

    public Transform playerCardDestination;
    public Image playerCardImage;
    #endregion

    #region Enemy UI
    public Slider enemySlider;
    public Image enemySliderColor;
    public TextMeshProUGUI enemyScoreText;
    public TextMeshProUGUI enemyMaxScoreText;
    public TextMeshProUGUI enemyTitleText;
    public TextMeshProUGUI enemyHPText;
    public Slider enemyHPBar;

    public Transform enemyCardDestination;
    public Image enemyCardImage;
    #endregion

    #region Game UI
    public TextMeshProUGUI deckCount;
    public TextMeshProUGUI bustChance;

    public Button standButton;
    #endregion

    void Start()
    {
        playerScoreText.text = "0";
        enemyScoreText.text = "0";

        playerSlider.value = 0;
        enemySlider.value = 0;
    }

    public void SetupUI(EnemyCurrent enemy, UnitData player, int playerDeckCount, int bustAmount)
    {
        enemyTitleText.text = enemy.Name;
        playerTitleText.text = player.Name;
        ChangeDeckCount(playerDeckCount);
        UpdateBustChance(bustAmount, playerDeckCount);
        UpdateMaxScore(Character.Player, player.MaxScore);
        UpdateMaxScore(Character.Enemy, enemy.MaxScore);
        SetMaxSliderValue(Character.Player, player.MaxScore);
        SetMaxSliderValue(Character.Enemy, enemy.MaxScore);
        SetUnitMaxHP(Character.Player, player.MaxHP);
        SetUnitMaxHP(Character.Enemy, enemy.CurrentHP);
    }

    public void SetMaxSliderValue(Character character, int value)
    {
        switch (character)
        {
            case Character.Player:
                playerSlider.maxValue = value;
                break;
            case Character.Enemy:
                enemySlider.maxValue = value;
                break;
        }
    }

    public void SetUnitMaxHP(Character unit, int hpValue)
    {
        switch (unit)
        {
            case Character.Player:
                playerHPBar.maxValue = hpValue;
                playerHPBar.value = hpValue;
                playerHPText.text = hpValue.ToString();
                break;
            case Character.Enemy:
                enemyHPBar.maxValue = hpValue;
                enemyHPBar.value = hpValue;
                enemyHPText.text = hpValue.ToString();
                break;
        }
    }

    public void UpdateStandUI(Character slide, CharacterStatus status, int newScore, int maxScore)
    {
        ChangeSlideValue(slide, status, newScore, maxScore);
    }

    public void UpdatePlayerInfo(int deckCount, int bustAmount)
    {
        ChangeDeckCount(deckCount);
        UpdateBustChance(bustAmount, deckCount);
    }

    public void UpdateMaxScore(Character unit, int newValue)
    {
        switch (unit)
        {
            case Character.Player:
                playerMaxScoreText.text = newValue.ToString();
                break;
            case Character.Enemy:
                enemyMaxScoreText.text = newValue.ToString();
                break;
        }
    }

    public void UpdateBustChance(int bustChanceAmount, int deckCount)
    {
        int successChanceValue = 100 - bustChanceAmount * 100 / deckCount;

        if (successChanceValue <= 30)
            bustChance.color = Color.red;
        else if (successChanceValue <= 60)
            bustChance.color = Color.yellow;
        else
            bustChance.color= Color.green;

        bustChance.text = $"Success: {successChanceValue}% ({bustChanceAmount}/{deckCount})";
    }

    public void ChangeSlideValue(Character slide, CharacterStatus status, int newValue, int maxScore)
    {
        switch (slide) {
            case Character.Player:

                playerSlider.value = newValue;
                playerScoreText.text = status == CharacterStatus.Bust ? $"{newValue}\nBUST!" : newValue.ToString();
                playerScoreText.color = newValue == maxScore ? Color.yellow : Color.cyan;
                playerSliderColor.color = newValue == maxScore ? Color.yellow : Color.cyan;
                break;
            case Character.Enemy:

                enemySlider.value = newValue;
                enemyScoreText.text = status == CharacterStatus.Bust ? $"{newValue}\nBUST!" : newValue.ToString();
                enemyScoreText.color = newValue == maxScore ? Color.yellow : Color.red;
                enemySliderColor.color = newValue == maxScore ? Color.yellow : Color.red;
                break;
        }
    }

    public void ChangeDeckCount(int newValue)
    {
        deckCount.text = $"Cards in deck: {newValue}";
    }

    public void ShowCardDrawn(Character character, GameCard card, EffectsManager animationManager, Action callback)
    {
        switch (character)
        {
            case Character.Player:
                HandleCardAnimation(animationManager, playerCardImage.transform, playerCardDestination, TypeOfObject.CardDrawnPlayer, playerCardImage, card.classId, card.id, callback);
                break;
            case Character.Enemy:
                HandleCardAnimation(animationManager, enemyCardImage.transform, enemyCardDestination, TypeOfObject.CardDrawnEnemy, enemyCardImage, card.classId, card.id, callback);
                break;
        }
    }

    void HandleCardAnimation(EffectsManager manager, Transform cardSource, Transform cardDestination, TypeOfObject type, Image card, int folderId, int cardId, Action callback)
    {
        card.gameObject.SetActive(true);
        card.sprite = Resources.Load<Sprite>($"Sprites/Cards/{folderId}/card_{cardId}");

        manager.StartMovement(cardSource, cardDestination, 5, type, callback);
    }

    public void DisableStandClick()
    {
        standButton.interactable = false;
    }
}
