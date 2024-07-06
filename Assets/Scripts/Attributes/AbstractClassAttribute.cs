using System;
using UnityEngine;

public class AbstractClassAttribute : PropertyAttribute
{
    public Type baseType;

    public AbstractClassAttribute(Type type)
    {
        baseType = type;
    }
}