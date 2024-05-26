using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager
{
    public List<Choice> choices;
    public Transform centerPoint;

    public ChoiceManager(List<Choice> choices, Transform centerPoint)
    {
        this.choices = choices;
        this.centerPoint = centerPoint;

        for(int i = 0; i < choices.Count; i++)
        {
            var choice = choices[i];
            var choiceTransform = choice.GetObject().transform;

            choice.AddCallback(ResetAllChoices);
            choice.GetObject().transform.SetPositionAndRotation(GameUIManager.GetCardPosition(i, choices.Count, centerPoint.position, 2f), GameUIManager.GetCardRotation(i, choices.Count, choiceTransform.rotation.x, choiceTransform.rotation.y));
        }
    }

    public void ResetAllChoices()
    {
        foreach (var choice in choices)
            choice.GetObject().SetActive(false);
    }

    public readonly struct Choice
    {
        readonly GameObject obj;
        readonly Button btn;
        public Choice(GameObject obj, string description, string iconName, ChoiceType type, object[] parameters, List<Action> callbackList)
        {
            this.obj = obj;
            btn = obj.GetComponent<Button>();
            SetupChoiceUI(obj, type, description, iconName, parameters, callbackList);
        }

        public GameObject GetObject()
        {
            return obj;
        }

        public readonly void AddCallback(Action callback)
        {
            btn.onClick.AddListener(() => callback());
        }

        public readonly void SetupChoiceUI(GameObject obj, ChoiceType type, string description, string iconName, object[] parameters, List<Action> callbackList)
        {
            btn.onClick.RemoveAllListeners();

            foreach (Action action in callbackList)
            {
                btn.onClick.AddListener(() => action());
            }

            HandleBackground(type, obj);
            HandleSpecificUI(type, obj, parameters);

            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = description;
            obj.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Icons/General/{iconName}");
            obj.SetActive(true);
        }

        void HandleBackground(ChoiceType type, GameObject obj)
        {
            switch (type)
            {
                case ChoiceType.ActionCard:
                    obj.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Cards/Card Structure/Civilian_card_version2");
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
                    var priceSection = obj.transform.Find("Price section");
                    priceSection.gameObject.SetActive(true);
                    priceSection.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = $"{parameters[0]}g";
                    break;
            }
        }
        
        public enum ChoiceType
        {
            Standard,
            ActionCard
        }
    }
}