using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ChoiceManager;

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
            choice.GetObject().transform.SetPositionAndRotation(GameUIManager.GetCardPosition(i, choices.Count, choiceTransform.position, 0.05f), GameUIManager.GetCardRotation(i, choices.Count, choiceTransform.rotation.x, choiceTransform.rotation.y));
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