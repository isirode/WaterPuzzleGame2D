using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class RequireComponent
{
    public static void RequireThrow<Result, IMonoBehaviour>(IMonoBehaviour monoBehaviour, Func<Result> expression) where IMonoBehaviour : MonoBehaviour
    {
        Result result = expression();
        // WARNING : for some reason, when the result is a GameObject (the only thing I've tested for now) and if the result is null
        // It will not be equal to null, nor the default of the type
        string resultAsString = result.ToString();
        if (result == null || resultAsString == "null")
        {
            throw new Exception($"Property of type {result.GetType()} is required in {monoBehaviour.GetType()} of name {monoBehaviour.name}. See the stacktrace to obtain the exact name of the property.");
        }
    }
}
