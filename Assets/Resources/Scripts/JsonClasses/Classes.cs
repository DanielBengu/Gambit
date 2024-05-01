using System.Collections.Generic;
using System;
using static CardsManager;

[Serializable]
public class ClassData
{
    public List<Class> Classes;
}

[Serializable]
public class Class
{
    public Classes Id;
    public string Name;
}