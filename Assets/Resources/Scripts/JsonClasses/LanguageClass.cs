using System;
using System.Collections.Generic;
using UnityEngine;
using static CardsManager;

[Serializable]
public class LanguageClass
{
    public List<LanguageStrings> Strings;
}

[Serializable]
public class LanguageStrings
{
    public int Id;
    public string Value;
}