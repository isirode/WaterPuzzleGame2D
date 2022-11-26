using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UQueryRequiredExtensions
{
    public static T QR<T>(this VisualElement e, string name = null, params string[] classes) where T : VisualElement
    {
        var result = e.Q<T>(name, classes);
        if (result == null)
        {
            throw new Exception($"Element of type {e.GetType()} not found inside {e.name} using name '{name}' and classes '{classes}'.");
        }
        return result;
    }
}
