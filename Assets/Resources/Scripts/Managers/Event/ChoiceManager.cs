using JetBrains.Annotations;
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

    public ChoiceManager(List<Choice> choices)
    {
        this.choices = choices;
        foreach (var choice in choices)
        {
            choice.AddCallback(ResetAllChoices);
        }
    }

    public void ResetAllChoices()
    {
        foreach (var choice in choices)
            choice.GetObject().SetActive(false);
    }

    public struct Choice
    {
        GameObject obj;
        Button btn;
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