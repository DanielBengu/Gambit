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

        foreach (var choice in choices)
        {
            choice.AddCallback(ResetAllChoices);
        }

        SetCardsPosition(choices.Select(c => c.GetObject().transform).ToList());
    }
    public void SetCardsPosition(List<Transform> cardPositions)
    {
        foreach (var card in cardPositions)
        {
            card.LookAt(centerPoint);
            card.SetPositionAndRotation(card.position, new Quaternion(0, 0, card.rotation.z, card.rotation.w));
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
        public Choice(GameObject obj, string description, string iconName, List<Action> callbackList)
        {
            this.obj = obj;
            btn = obj.GetComponent<Button>();
            SetupChoiceUI(obj, description, iconName, callbackList);
        }

        public GameObject GetObject()
        {
            return obj;
        }

        public readonly void AddCallback(Action callback)
        {
            btn.onClick.AddListener(() => callback());
        }

        public readonly void SetupChoiceUI(GameObject obj, string description, string iconName, List<Action> callbackList)
        {
            btn.onClick.RemoveAllListeners();

            foreach (Action action in callbackList)
            {
                btn.onClick.AddListener(() => action());
            }

            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = description;
            obj.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Sprites/Icons/{iconName}");
            obj.SetActive(true);
        }
    }
}