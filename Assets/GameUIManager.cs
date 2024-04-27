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
    public TextMeshProUGUI playerArmorText;
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
    public TextMeshProUGUI enemyArmorText;
    public Slider enemyHPBar;

    public Transform enemyCardDestination;
    public Image enemyCardImage;
    #endregion

    #region Game UI
    public TextMeshProUGUI deckCount;
    public TextMeshProUGUI bustChance;

    public Button standButton;

    public GameObject enemyInfoUI;
    public GameObject hitOrStandUI;
    public GameObject scoreUI;

    public Image blackScreen;
    #endregion

    void Start()
    {
        playerScoreText.text = "0";
        enemyScoreText.text = "0";

        playerSlider.value = 0;
        enemySlider.value = 0;
    }

    public void SetupBlackScreen(bool active, EffectsManager effectsManager)
    {
        blackScreen.gameObject.SetActive(active);

        Color resetColor = blackScreen.color;
        resetColor.a = 1f; // Reset alpha to fully opaque
        blackScreen.color = resetColor;

        if (active)
            effectsManager.effects.Add(new()
            {
                obj = blackScreen.gameObject,
                effect = EffectsManager.Effects.FightStartup,
                callback = () => { }
            });
    }

    public void SetupUI(EnemyCurrent enemy, UnitData player, int playerDeckCount, int bustAmount)
    {
        enemyTitleText.text = enemy.Name;
        playerTitleText.text = player.Name;
        ChangeDeckCount(playerDeckCount);
        UpdateBustChance(bustAmount, playerDeckCount);

        UpdateMaxScore(Character.Player, player.MaxScore);
        SetMaxSliderValue(Character.Player, player.MaxScore);
        SetUnitHP(Character.Player, player.CurrentHP, player.MaxHP);
        UpdateArmor(Character.Player, player.Armor);

        SetUnitHP(Character.Enemy, enemy.CurrentHP, enemy.MaxHP);
        UpdateArmor(Character.Enemy, enemy.Armor);
        SetMaxSliderValue(Character.Enemy, enemy.MaxScore);
        UpdateMaxScore(Character.Enemy, enemy.MaxScore);
    }

    public void UpdateArmor(Character character, int newValue)
    {
        switch (character)
        {
            case Character.Enemy:
                enemyArmorText.text = newValue.ToString();
                break;
            case Character.Player:
                playerArmorText.text = newValue.ToString();
                break;
        }
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

    public void SetUnitHP(Character unit, int hpValue, int maxHpValue)
    {
        string hpString = GetHPString(hpValue, maxHpValue);

        switch (unit)
        {
            case Character.Player:
                playerHPBar.maxValue = maxHpValue;
                playerHPBar.value = hpValue;
                playerHPText.text = hpString;
                break;
            case Character.Enemy:
                enemyHPBar.maxValue = maxHpValue;
                enemyHPBar.value = hpValue;
                enemyHPText.text = hpString;
                break;
        }
    }

    string GetHPString(int currentHp, int maxHp)
    {
        return $"health {currentHp}/{maxHp}"; 
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
        if(deckCount == 0)
        {
            bustChance.text = "Success: ???";
            return;
        }

        int successChanceValue = 100 - bustChanceAmount * 100 / deckCount;

        if (successChanceValue <= 30)
            bustChance.color = Color.red;
        else if (successChanceValue <= 60)
            bustChance.color = Color.yellow;
        else
            bustChance.color= Color.green;

        bustChance.text = $"Success: {successChanceValue}% ({bustChanceAmount}/{deckCount})";
    }

    public void TurnOfFightUI()
    {
        enemyInfoUI.SetActive(false);
        hitOrStandUI.SetActive(false);
        scoreUI.SetActive(false);
    }

    public void ChangeSlideValue(Character slide, CharacterStatus status, int newValue, int maxScore)
    {
        switch (slide) {
            case Character.Player:

                playerSlider.value = newValue;
                playerScoreText.text = status == CharacterStatus.Bust ? $"{newValue}\nBUST!" : newValue.ToString();
                playerScoreText.color = newValue == maxScore ? Color.yellow : Color.white;
                playerSliderColor.color = newValue == maxScore ? Color.yellow : Color.white;
                break;
            case Character.Enemy:

                enemySlider.value = newValue;
                enemyScoreText.text = status == CharacterStatus.Bust ? $"{newValue}\nBUST!" : newValue.ToString();
                enemyScoreText.color = newValue == maxScore ? Color.yellow : Color.white;
                enemySliderColor.color = newValue == maxScore ? Color.yellow : Color.white;
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

    public void SetStandButtonInteractable(bool interactable)
    {
        standButton.interactable = interactable;
    }
}
