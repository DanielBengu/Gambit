using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FightManager;

public class GameUIManager : MonoBehaviour
{
    public Slider playerSlider;
    public Image playerSliderColor;
    public TextMeshProUGUI playerScoreText;

    public Slider enemySlider;
    public Image enemySliderColor;
    public TextMeshProUGUI enemyScoreText;

    public TextMeshProUGUI deckCount;
    public TextMeshProUGUI bustChance;

    public Image cardImage;

    public TextMeshProUGUI enemyTitleText;

    public Button standButton;

    void Start()
    {
        playerScoreText.text = "0";
        enemyScoreText.text = "0";

        playerSlider.value = 0;
        enemySlider.value = 0;
    }

    public void SetupUI(string enemyTitle, int playerDeckCount, int bustAmount)
    {
        enemyTitleText.text = enemyTitle;
        ChangeDeckCount(playerDeckCount);
        UpdateBustChance(bustAmount, playerDeckCount, CharacterStatus.Playing);
    }

    public void UpdateUI(Character slide, int newScore, int maxScore, int currentDeckCount, int bustAmount, CharacterStatus status)
    {
        ChangeSlideValue(slide, newScore, maxScore);
        ChangeDeckCount(currentDeckCount);
        UpdateBustChance(bustAmount, currentDeckCount, status);
    }

    public void UpdateBustChance(int bustChanceAmount, int deckCount, CharacterStatus status)
    {
        if(status != CharacterStatus.Playing)
        {
            bustChance.gameObject.SetActive(false);
            return;
        }

        bustChance.text = $"Cards that busts: {bustChanceAmount}/{deckCount} ({bustChanceAmount * 100 / deckCount}%)";
    }

    public void ChangeSlideValue(Character slide, int newValue, int maxScore)
    {
        bool isBust = newValue > maxScore;
        switch (slide) {
            case Character.Player:
                if (!isBust)
                    playerSlider.value = newValue;

                playerScoreText.text = isBust ? "BUST!" : newValue.ToString();
                playerScoreText.color = newValue == maxScore ? Color.yellow : Color.cyan;

                playerSliderColor.color = newValue == maxScore ? Color.yellow : Color.cyan;
                break;
            case Character.Enemy:
                if(!isBust)
                    enemySlider.value = newValue;

                enemyScoreText.text = isBust ? "BUST!" : newValue.ToString();
                enemyScoreText.color = newValue == maxScore ? Color.yellow : Color.red;

                enemySliderColor.color = newValue == maxScore ? Color.yellow : Color.red;
                break;
        }
    }

    public void ChangeDeckCount(int newValue)
    {
        deckCount.text = newValue.ToString();
    }

    public void ShowCardDrawn(GameCard card)
    {
        cardImage.gameObject.SetActive(true);

        //Should be improved and pre-loaded
        cardImage.sprite = Resources.Load<Sprite>($"Sprites/Cards/{card.classId}/card_{card.id}");
    }

    public void DisableStandClick()
    {
        standButton.interactable = false;
    }
}
