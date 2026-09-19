using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class StartValuation
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StartModifier> modifiers = new List<StartModifier>();
    private bool isModifier = true;
    private float finalValue;

    public float GetValue()
    {
        if (isModifier)
        {
            finalValue = GetFinalValue();
            isModifier = false;
        }

        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StartModifier modToAdd = new StartModifier(value, source);
        modifiers.Add(modToAdd);
        isModifier = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        isModifier = true;
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach (var modifier in modifiers)
        {
            finalValue += modifier.value;
        }

        return finalValue;
    }

    public float GetBaseValue() => baseValue;

    public void SetBaseValue(float value)
    {
        baseValue = value;
        isModifier = true;
    }
}


[Serializable]
public class StartModifier
{
    public float value;
    public string source;

    public StartModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
