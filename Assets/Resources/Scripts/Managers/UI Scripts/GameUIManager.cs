using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static VisualEffectsManager.MovingObject;
using static FightManager;
using static CardsManager;
using static UnityEngine.EventSystems.EventTrigger;

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

    #region Fight UI

    public GameObject fightUI;

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

    #endregion

    #region Event UI

    public GameObject eventUI;

    #endregion

    public LanguageManager languageManager;

    void Start()
    {
        playerScoreText.text = "0";
        enemyScoreText.text = "0";

        playerSlider.value = 0;
        enemySlider.value = 0;
    }

    public void SetupBlackScreen(bool active, VisualEffectsManager effectsManager, Action callback)
    {
        blackScreen.gameObject.SetActive(active);

        Color resetColor = blackScreen.color;
        resetColor.a = 1f; // Reset alpha to fully opaque
        blackScreen.color = resetColor;

        if (active)
            effectsManager.effects.Add(new()
            {
                obj = blackScreen.gameObject,
                effect = VisualEffectsManager.Effects.FightStartup,
                callback = callback
            });
    }

    public void SetupFightUI(FightUnit enemy, FightUnit player, int playerDeckCount, int bustAmount)
    {
        fightUI.SetActive(true);

        SetUnitTitle(enemyTitleText, enemy.Name, enemy.Class.Class.ToString());
        SetUnitTitle(playerTitleText, player.Name, player.Class.Class.ToString());

        ChangeDeckCount(playerDeckCount);
        UpdateBustChance(bustAmount, playerDeckCount);

        UpdateMaxScore(Character.Player, player.CurrentMaxScore);
        SetMaxSliderValue(Character.Player, player.CurrentMaxScore);
        UpdateUnitHP(Character.Player, player.FightHP, player.FightMaxHP);
        UpdateArmor(Character.Player, player.FightArmor);

        UpdateUnitHP(Character.Enemy, enemy.FightHP, enemy.FightMaxHP);
        UpdateArmor(Character.Enemy, enemy.FightArmor);
        SetMaxSliderValue(Character.Enemy, enemy.CurrentMaxScore);
        UpdateMaxScore(Character.Enemy, enemy.CurrentMaxScore);
    }

    public void SetPlayerSection(string name, string className, int maxHp, int currentHp, int armor)
    {
        SetUnitTitle(playerTitleText, name, className);
        UpdateArmor(Character.Player, armor);
        UpdateUnitHP(Character.Player, currentHp, maxHp);
    }

    public void SetUnitTitle(TextMeshProUGUI textObj, string name, string unitClass)
    {
        languageManager.SetLanguageValues(new()
        {
            new(7, textObj, new object[2]{ name, unitClass }),
        });
    }

    public void SetCardImage(GameObject card, IClass cardClass, CardType cardType)
    {
        Sprite cardIcon = cardClass.GetCardIcon(cardType);
        string cardText = cardClass.GetCardText(cardType);

        Transform cardIconTransform = card.transform.Find("Icon");

        cardIconTransform.gameObject.SetActive(cardIcon != null);
        cardIconTransform.GetComponent<Image>().sprite = cardIcon;

        card.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = cardText;
    }

    public void SetupEventUI()
    {
        eventUI.SetActive(true);
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

    public void UpdateUnitHP(Character unit, int hpValue, int maxHpValue)
    {
        switch (unit)
        {
            case Character.Player:
                playerHPBar.maxValue = maxHpValue;
                playerHPBar.value = hpValue;

                if(hpValue <= 0)
                    languageManager.SetLanguageValues(new()
                    {
                        new(9, playerHPText, new object[0]{}),
                    });
                else
                {
                    languageManager.SetLanguageValues(new()
                    {
                        new(10, playerHPText, new object[2]{hpValue, maxHpValue}),
                    });
                }
                break;
            case Character.Enemy:
                enemyHPBar.maxValue = maxHpValue;
                enemyHPBar.value = hpValue;

                if (hpValue <= 0)
                    languageManager.SetLanguageValues(new()
                    {
                        new(9, enemyHPText, new object[0]{}),
                    });
                else
                {
                    languageManager.SetLanguageValues(new()
                    {
                        new(10, enemyHPText, new object[2]{hpValue, maxHpValue}),
                    });
                }
                break;
        }
    }

    public void UpdateUI(Character character, FightUnit data)
    {
        UpdateStandUI(character, data.status, data.currentScore, data.CurrentMaxScore);

        UpdateUnitHP(character, data.FightHP, data.FightMaxHP);
        UpdateArmor(character, data.FightArmor);

        if (character == Character.Player)
            UpdatePlayerInfo(data.FightCurrentDeck.Count, GetCardsBustAmount(data.FightCurrentDeck, data.currentScore, data.CurrentMaxScore));
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
            languageManager.SetLanguageValues(new()
            {
                new(6, bustChance, new object[0]),
            });
            return;
        }

        int successChanceValue = 100 - bustChanceAmount * 100 / deckCount;

        if (successChanceValue <= 30)
            bustChance.color = Color.red;
        else if (successChanceValue <= 60)
            bustChance.color = Color.yellow;
        else
            bustChance.color= Color.green;

        languageManager.SetLanguageValues(new()
        {
            new(5, bustChance, new object[3]{ successChanceValue, deckCount - bustChanceAmount, deckCount }),
        });
    }

    public void TurnOfFightUI()
    {
        enemyInfoUI.SetActive(false);
        hitOrStandUI.SetActive(false);
        scoreUI.SetActive(false);
    }

    public void TurnOffEventUI()
    {
        eventUI.SetActive(false);
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
        languageManager.SetLanguageValues(new()
        {
            new(8, deckCount, new object[1]{ newValue }),
        });
    }

    public void ShowCardDrawn(Character character, GameCard card, IClass unitClass, VisualEffectsManager animationManager, Action callback)
    {
        switch (character)
        {
            case Character.Player:
                HandleCardAnimation(animationManager, playerCardImage.transform, playerCardDestination, TypeOfObject.CardDrawnPlayer, unitClass, card.cardType, callback);
                break;
            case Character.Enemy:
                HandleCardAnimation(animationManager, enemyCardImage.transform, enemyCardDestination, TypeOfObject.CardDrawnEnemy, unitClass, card.cardType, callback);
                break;
        }
    }

    void HandleCardAnimation(VisualEffectsManager manager, Transform cardSource, Transform cardDestination, TypeOfObject type, IClass unitClass, CardType cardType, Action callback)
    {
        cardSource.gameObject.SetActive(true);

        SetCardImage(cardSource.gameObject, unitClass, cardType);

        manager.StartMovement(cardSource, cardDestination, 5, type, callback);
    }

    public void SetStandButtonInteractable(bool interactable)
    {
        standButton.interactable = interactable;
    }
}
