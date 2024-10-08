using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static VisualEffectsManager.MovingObject;
using static FightManager;
using System.Collections.Generic;
using Unity.VisualScripting;
using static MenuOptions;
using System.Collections;
using Assets.Resources.Scripts.Fight;

public class GameUIManager : MonoBehaviour
{
    #region Player UI
    public Slider playerSlider;
    public Image playerSliderColor;
    public Image playerSliderBackground;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI playerMaxScoreText;
    public TextMeshProUGUI playerTitleText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerArmorText;
    public Slider playerHPBar;

    public Transform playerCardDestination;
    public Image playerCardImage;

    #region Gold Variables

    public TextMeshProUGUI goldAmountText;

    Vector3 originalPosition;
    Color originalColor;
    float originalFontSize;

    bool isGoldShaking = false;

    #endregion

    #endregion

    #region Fight UI

    public GameObject fightUI;

    #region Enemy UI
    public Slider enemySlider;
    public Image enemySliderColor;
    public Image enemySliderBackground;
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
    public TextMeshProUGUI previsionText;

    public TextMeshProUGUI gameDeckCount;

    public Button standButton;

    public GameObject enemyInfoUI;
    public GameObject hitOrStandUI;
    public GameObject scoreUI;

    public Image blackScreen;
    public Image previsionSword;

    public GameObject actionCardPrefab;
    public GameObject cardSection;

    public Animator scoreDeckGlowVFX;
    public Animator scoreDeckBouncer;
    public Animator standGlowVFX;
    public Animator standBouncer;

    public Transform discardPosition;

    public Animator playerArmorAnimator;
    public Animator enemyArmorAnimator;

    #endregion

    #endregion

    #region Event UI

    public GameObject eventUI;

    #endregion

    public LanguageManager languageManager;

    #region Info Panel

    public GameObject infoPanel;

    #endregion

    void Start()
    {
        playerScoreText.text = "0";
        enemyScoreText.text = "0";

        playerSlider.value = 0;
        enemySlider.value = 0;
    }

    public void SetupBlackScreen(bool active, VisualEffectsManager.Effects effect, VisualEffectsManager effectsManager, Action callback)
    {
        blackScreen.gameObject.SetActive(active);

        Color resetColor = blackScreen.color;
        resetColor.a = effect == VisualEffectsManager.Effects.LightenBlackScreen ? 1f : 0.2f; // Reset alpha to starting opacity
        blackScreen.color = resetColor;

        if (active)
            effectsManager.effects.Add(new()
            {
                obj = blackScreen.gameObject,
                effect = effect,
                callback = new() { callback }
            });
    }

    public void SetupFightUI(FightUnit enemy, FightUnit player)
    {
        fightUI.SetActive(true);
        enemyInfoUI.SetActive(true);
        hitOrStandUI.SetActive(true);
        scoreUI.SetActive(true);
        cardSection.SetActive(true);

        SetUnitTitle(enemyTitleText, enemy.Name, enemy.Class.Class.ToString());

        UpdateMaxScore(Character.Player, player.CurrentMaxScore);
        SetMaxSliderValue(Character.Player, player.CurrentMaxScore);

        SetMaxSliderValue(Character.Enemy, enemy.CurrentMaxScore);
        UpdateMaxScore(Character.Enemy, enemy.CurrentMaxScore);

        UpdateUI(player, true);
        UpdateUI(enemy, true);
    }

    public void UpdateHand(List<ActionCard> currentHand, FightManager manager)
    {
        ClearHand();

        foreach (var card in currentHand)
        {
            AddCardToHand(card, currentHand.IndexOf(card), currentHand.Count, manager);
        }
    }

    public void ClearHand()
    {
        for (int i = cardSection.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cardSection.transform.GetChild(i).gameObject);
        }
    }

    public void AddCardToHand(ActionCard card, int positionInHand, int cardsInHand, FightManager manager)
    {
        Vector3 cardPosition = GetCardPosition(positionInHand, cardsInHand, cardSection.transform.position, 1f);
        Quaternion newRotation = GetCardRotation(positionInHand, cardsInHand, cardSection.transform.rotation.x, cardSection.transform.rotation.y);

        InstantiateActionCard(card, cardPosition, newRotation, cardSection.transform, manager, true, manager.Player, manager.Enemy);
    }

    public GameObject InstantiateActionCard(ActionCard card, Vector3 cardPosition, Quaternion newRotation, Transform parent, FightManager manager, bool isFunctional, FightUnit unit, FightUnit enemy)
    {
        var newCard = Instantiate(actionCardPrefab, cardPosition, newRotation, parent);
        UpdateActionCardUI(newCard, card, unit, enemy);

        var script = newCard.GetComponent<ActionCardInstance>();

        if (isFunctional)
            script.LoadActionCard(card, manager);
        else
            Destroy(script);

        return newCard;
    }

    public static Vector3 GetCardPosition(int position, int cardNumber, Vector3 basePosition, float scale)
    {
        float vertexPositionX = (cardNumber - 1) / 2f;
        float x = position - vertexPositionX;
        float c = - x * 10;

        float cardPositionX = basePosition.x + (x * 1.5f * scale);
        float cardPositionY = basePosition.y - (Math.Abs(c) / 22 * scale);

        return new(cardPositionX, cardPositionY, basePosition.z);
    }

    public static Quaternion GetCardRotation(int position, int cardNumber, float rotationX, float rotationY)
    {
        float vertexPositionX = (cardNumber - 1) / 2f;
        float x = position - vertexPositionX;
        float c = - x * 10;

        return Quaternion.Euler(rotationX, rotationY, c);
    }

    void UpdateActionCardUI(GameObject cardObj, ActionCard card, FightUnit unit, FightUnit enemy)
    {
        cardObj.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Icons/ActionCard/{card.Id}");

        var descriptionParameters = ActionCardArchive.GetActionCardText(card.Id, unit, enemy);

        languageManager.SetLanguageValues(new()
        {
            new(card.NameIdValue, cardObj.transform.Find("Title").GetComponent<TextMeshProUGUI>(), new object[0]{ }),
        });

        languageManager.SetLanguageValues(new()
        {
            new(card.DescriptionIdValue, cardObj.transform.Find("Effect").GetComponent<TextMeshProUGUI>(), descriptionParameters),
        });

        //cardObj.GetComponent<Image>().color = IClass.GetCardBackgroundColor(card.ClassId);
    }

    public void SetPlayerSection(int maxHp, int currentHp, int armor)
    {
        //SetUnitTitle(playerTitleText, name, className);
        UpdateArmor(Character.Player, armor, false);
        UpdateUnitHP(Character.Player, currentHp, maxHp);
    }

    public void UpdateGoldAmount(int gold)
    {
        languageManager.SetLanguageValues(new()
        {
            new(54, goldAmountText, new object[1] {gold})
        });
    }

    public void SetUnitTitle(TextMeshProUGUI textObj, string name, string unitClass)
    {
        languageManager.SetLanguageValues(new()
        {
            new(7, textObj, new object[2]{ name.ToUpper(), unitClass }),
        });
    }

    public void SetCardImage(GameObject cardObj, GameCard card, IClass cardClass)
    {
        Sprite cardIcon = cardClass.GetCardIcon(card);

        cardObj.GetComponent<Image>().sprite = cardIcon;
    }

    public void SetupEventUI()
    {
        eventUI.SetActive(true);
    }

    public void UpdateArmor(Character character, int newValue, bool startAnimation)
    {
        Animator animatorToPlay = playerArmorAnimator;
        int difference = 0;

        switch (character)
        {
            case Character.Enemy:
                animatorToPlay = enemyArmorAnimator;
                difference = int.Parse(enemyArmorText.text) - newValue;

                enemyArmorText.text = newValue.ToString();
                break;
            case Character.Player:
                animatorToPlay = playerArmorAnimator;
                difference = int.Parse(playerArmorText.text) - newValue;

                playerArmorText.text = newValue.ToString();
                break;
        }

        if(startAnimation && difference != 0)
            StartArmorChangeAnimation(animatorToPlay, difference);
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

    public void UpdateUI(FightUnit data, bool isFightStart = false)
    {
        Character character = data.Character;

        UpdateStandUI(character, data.status, data.currentScore, data.CurrentMaxScore, data.Attacks);

        UpdateUnitHP(character, data.FightHP, data.FightMaxHP);
        UpdateArmor(character, data.FightArmor, !isFightStart);

        if (character == Character.Player)
            UpdatePlayerInfo(data.FightCurrentDeck.Count, GetCardsBustAmount(data.FightCurrentDeck, data.currentScore, data.CurrentMaxScore), data.FightActionCurrentDeck.Count);
    }

    public void UpdateStandUI(Character slide, CharacterStatus status, int newScore, int maxScore, int attacks)
    {
        ChangeSlideValue(slide, status, newScore, maxScore, attacks);
    }

    public void UpdatePrevision(PrevisionEnum prevision, string damageAmount)
    {
        switch (prevision)
        {
            case PrevisionEnum.PlayerAdvantage:
                previsionSword.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case PrevisionEnum.Tie:
                previsionSword.transform.rotation = Quaternion.Euler(0, 180, -45);
                break;
            case PrevisionEnum.EnemyAdvantage:
                previsionSword.transform.rotation = Quaternion.Euler(0, 180, -90);
                break;
        }

        previsionText.text = damageAmount;
    }

    public void UpdatePlayerInfo(int deckCount, int bustAmount, int gameDeckCount)
    {
        ChangeDeckCount(deckCount);
        UpdateBustChance(bustAmount, deckCount);

        UpdateGameDeckCount(gameDeckCount);
    }

    public void UpdateGameDeckCount(int deckCount)
    {
        gameDeckCount.text = deckCount.ToString();
    }

    public void UpdateMaxScore(Character unit, int newValue)
    {
        switch (unit)
        {
            case Character.Player:
                playerMaxScoreText.text = $"/{newValue}";
                break;
            case Character.Enemy:
                enemyMaxScoreText.text = $"/{newValue}";
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
        fightUI.SetActive(false);
    }

    public void TurnOffEventUI()
    {
        eventUI.SetActive(false);
    }

    public void ChangeSlideValue(Character slide, CharacterStatus status, int newValue, int maxScore, int attacks)
    {
        switch (slide) {
            case Character.Player:
                Color color = newValue == maxScore ? Color.yellow : Color.cyan;
                playerSlider.value = newValue;
                playerScoreText.text = GetScoreText(status, newValue, attacks);
                playerScoreText.color = color;
                playerSliderColor.color = color;
                playerSliderBackground.color = color;
                break;
            case Character.Enemy:
                Color enemyColor = newValue == maxScore ? Color.yellow : Color.red;
                enemySlider.value = newValue;
                enemyScoreText.text = GetScoreText(status, newValue, attacks);
                enemyScoreText.color = enemyColor;
                enemySliderColor.color = enemyColor;
                enemySliderBackground.color = enemyColor;
                break;
        }
    }

    public void StartArmorChangeAnimation(Animator animator, int valueModifier) {
        animator.Play("Armor_Change_Start");
    }


    public string GetScoreText(CharacterStatus status, int newValue, int attacks)
    {
        if(status == CharacterStatus.Bust)
        {
            return $"{newValue}\nBUST!";
        }

        if (attacks <= 0)
            return "0";
        else if (attacks == 1)
            return newValue.ToString();
        else
            return $"{newValue}x{attacks}";
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
        if (card == null)
        {
            callback();
            return;
        }

        switch (character)
        {
            case Character.Player:
                HandleCardAnimation(animationManager, playerCardImage.transform, playerCardDestination, TypeOfObject.CardDrawnPlayer, callback, 5);
                SetCardImage(playerCardImage.gameObject, card, unitClass);
                break;
            case Character.Enemy:
                HandleCardAnimation(animationManager, enemyCardImage.transform, enemyCardDestination, TypeOfObject.CardDrawnEnemy, callback, 5);
                SetCardImage(enemyCardImage.gameObject, card, unitClass);
                break;
        }

    }

    public void SendActionCardToGraveyardAndUpdateHand(VisualEffectsManager manager, Transform transform, List<ActionCard> cardList, FightManager fightManager)
    {
        HandleCardAnimation(manager, transform, discardPosition, TypeOfObject.ActionCardDiscarded, () => UpdateHand(cardList, fightManager), 20);
    }

    void HandleCardAnimation(VisualEffectsManager manager, Transform cardSource, Transform cardDestination, TypeOfObject type, Action callback, int speed)
    {
        cardSource.gameObject.SetActive(true);

        manager.StartMovement(cardSource, cardDestination, speed, type, callback);
    }

    public void SetStandButtonInteractable(bool interactable)
    {
        standButton.interactable = interactable;
    }

    public void HandleInsufficientFunds()
    {
        if (isGoldShaking)
            return;

        originalColor = goldAmountText.color;
        originalFontSize = goldAmountText.fontSize;
        originalPosition = goldAmountText.transform.localPosition;
        isGoldShaking = true;

        StartCoroutine(AnimateInsufficientFunds());
    }

    private IEnumerator AnimateInsufficientFunds()
    {
        // Change the color to red
        goldAmountText.color = Color.red;

        // Increase the font size
        goldAmountText.fontSize += 10;

        float shakeDuration = 0.5f;
        float shakeMagnitude = 10f;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Shake the text by changing its position slightly
            float offsetX = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = UnityEngine.Random.Range(-shakeMagnitude, shakeMagnitude);
            goldAmountText.rectTransform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isGoldShaking = false;

        // Revert the text to its original properties
        goldAmountText.color = originalColor;
        goldAmountText.fontSize = originalFontSize;
        goldAmountText.rectTransform.localPosition = originalPosition;
    }

    public void ChangeGlowVFX(Animator bouncer, Animator glowVFX, bool glow)
    {
        if (glow)
        {
            glowVFX.Play("Enabled");
            bouncer.Play("Idle_float");
        }
        else
        {
            glowVFX.Play("Disabled");
            bouncer.Play("Idle_disabled");
        }
    }

    #region Info Panel

    public void LoadActionDeckInfo(List<ActionCard> actionCards, FightManager manager)
    {
        GameObject objParent = infoPanel.transform.Find("Objects").gameObject;
        Vector3 basePosition = infoPanel.transform.Find("Scroller").transform.position;
        foreach (var card in actionCards)
        {
            Vector3 newPositions = GameManager.GetItemPositionOnInfoPanelList(basePosition, actionCards.IndexOf(card));
            var cardInstance = InstantiateActionCard(card, newPositions, objParent.transform.rotation, objParent.transform, manager, false, manager.Player, manager.Enemy);
            float scale = 1.75f;
            cardInstance.transform.localScale = new Vector3(cardInstance.transform.localScale.x * scale, cardInstance.transform.localScale.y * scale, cardInstance.transform.localScale.z * scale);
        }
    }

    public void ClearInfoPanel()
    {
        GameObject objParent = infoPanel.transform.Find("Objects").gameObject;
        for (int i = 0; i < objParent.transform.childCount; i++)
        {
            Destroy(objParent.transform.GetChild(i).gameObject);
        }
    }

    #endregion

    public enum PrevisionEnum
    {
        PlayerAdvantage,
        Tie,
        EnemyAdvantage
    }
}
