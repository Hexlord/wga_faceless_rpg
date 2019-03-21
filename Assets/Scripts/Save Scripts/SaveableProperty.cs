using System;


[AttributeUsage(AttributeTargets.Field)]
public class Saveable : Attribute
{
    public Saveable(string name)
    {
        Name = name;
    }
    public string Name
    {
        get; private set;
    }
}
