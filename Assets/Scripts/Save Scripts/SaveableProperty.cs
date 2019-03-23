using System;


[AttributeUsage(AttributeTargets.Field)]
public class Saveable : Attribute
{
    public Saveable()
    {
        Tag = "";
    }
    public Saveable(string tag)
    {
        Tag = tag;
    }
    public string Tag
    {
        get; private set;
    }
}
