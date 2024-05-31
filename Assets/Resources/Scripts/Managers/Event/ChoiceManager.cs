using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChoiceManager.Choice;

public class ChoiceManager
{
    public List<Choice> choices;
    public Transform centerPoint;

    public GameObject standardChoice;
    public GameObject actionCardChoice;
    public GameObject scoreCardChoice;

    public Transform choiceParent;

    readonly List<GameObject> choicesObj = new();

    public ChoiceManager(List<Choice> choices, Transform centerPoint)
    {
        this.choices = choices;
        this.centerPoint = centerPoint;

        choiceParent = GameObject.Find("Choices").transform;
        standardChoice = Resources.Load<GameObject>("Prefabs/Event/EventChoice");
        actionCardChoice = Resources.Load<GameObject>("Prefabs/Event/ActionCardChoice");
        scoreCardChoice = Resources.Load<GameObject>("Prefabs/Event/ScoreCardChoice");

        for (int i = 0; i < choices.Count; i++)
        {
            var choice = choices[i];

            GameObject choiceObj = LoadChoice(choice.choice);

            choicesObj.Add(choiceObj);

            LoadChoiceCallback(choiceObj, choice.callbackList);
            SetupChoiceUI(choiceObj, choice.choice, choice.description, choice.iconName, choice.parameters);

            choiceObj.transform.SetPositionAndRotation(GameUIManager.GetCardPosition(i, choices.Count, centerPoint.position, 1.7f), GameUIManager.GetCardRotation(i, choices.Count, choiceObj.transform.rotation.x, choiceObj.transform.rotation.y));
        }
    }

    public GameObject LoadChoice(ChoiceType choice)
    {
        return choice switch
        {
            ChoiceType.Standard => UnityEngine.Object.Instantiate(standardChoice, choiceParent),
            ChoiceType.ActionCard => UnityEngine.Object.Instantiate(actionCardChoice, choiceParent),
            ChoiceType.ScoreCard => UnityEngine.Object.Instantiate(scoreCardChoice, choiceParent),
            _ => GetMissingChoiceType(choice)
        };
    }

    GameObject GetMissingChoiceType(ChoiceType type)
    {
        Debug.LogError($"ERROR DURING CARD CREATION - CHOICE {type} NOT FOUND");
        return null;
    }

    public void LoadChoiceCallback(GameObject obj, List<Action> callbackList)
    {
        var btn = obj.GetComponent<Button>();

        btn.onClick.RemoveAllListeners();

        foreach (Action action in callbackList)
        {
            btn.onClick.AddListener(() => action());
        }
    }

    public void SetupChoiceUI(GameObject obj, ChoiceType type, string description, string iconName, object[] parameters)
    {
        HandleBackground(type, obj, parameters);
        HandleSpecificUI(type, obj, parameters);

        obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = description;
        obj.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Icons/General/{iconName}");
        obj.SetActive(true);
    }

    public void ResetAllChoices()
    {
        for (int i = 0; i < choicesObj.Count; i++)
        {
            var choice = choicesObj[i];

            UnityEngine.Object.Destroy(choice);
            choicesObj.Remove(choice);
            i--;
        } 
    }

    void HandleBackground(ChoiceType type, GameObject obj, object[] parameters)
    {
        switch (type)
        {
            case ChoiceType.ActionCard:
                obj.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Cards/Card Structure/Civilian_card_version2");
                break;
            case ChoiceType.ScoreCard:
                obj.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Cards/{parameters[1]}");
                break;
            default:
                break;
        }
    }

    void HandleSpecificUI(ChoiceType type, GameObject obj, object[] parameters)
    {
        switch (type)
        {
            case ChoiceType.Standard:
                break;
            case ChoiceType.ActionCard:
            case ChoiceType.ScoreCard:
                var priceSection = obj.transform.Find("Price section");
                priceSection.gameObject.SetActive(true);
                priceSection.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"{parameters[0]}g";
                break;
        }
    }

    public readonly struct Choice
    {
        public readonly ChoiceType choice;
        public readonly List<Action> callbackList;
        public readonly string description;
        public readonly string iconName;
        public readonly object[] parameters;

        public Choice(string description, string iconName, ChoiceType type, object[] parameters, List<Action> callbackList)
        {
            choice = type;
            this.description = description;
            this.iconName = iconName;
            this.callbackList = callbackList;
            this.parameters = parameters;
        }
        
        public enum ChoiceType
        {
            Standard,
            ActionCard,
            ScoreCard
        }
    }
}