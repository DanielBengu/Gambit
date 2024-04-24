using System.Collections.Generic;
using System;

[Serializable]
public class ClassData
{
    public List<Class> Classes;
}

[Serializable]
public class Class
{
    public int Id;
    public string Name;
}