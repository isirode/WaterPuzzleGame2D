using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class RequireComponent
{
    // TODO : there is probably ways to obtain the name of the variable
    public static void RequireThrow<Result, IMonoBehaviour>(IMonoBehaviour monoBehaviour, Result variable) where IMonoBehaviour : MonoBehaviour
    {
        void doThrow() => throw new Exception($"Property of type {variable.GetType()} is required in {monoBehaviour.GetType()} of name {monoBehaviour.name}. See the stacktrace to obtain the exact name of the property.");
        if (variable == null)
        {
            doThrow();
        }
        else
        {
            // WARNING : for some reason, when the result is a GameObject (the only thing I've tested for now) and if the result is null
            // It will not be equal to null, nor the default of the type
            // FIXME : this does not work with actually null object
            string resultAsString = variable.ToString();
            if (resultAsString == "null")
            {
                doThrow();
            }
        }
    }

    public static void RequireNotEmptyThrow<IMonoBehaviour>(IMonoBehaviour monoBehaviour, string variable) where IMonoBehaviour : MonoBehaviour
    {
        if (string.IsNullOrEmpty(variable))
        {
            throw new Exception($"Property of type {variable.GetType()} is required in {monoBehaviour.GetType()} of name {monoBehaviour.name}. See the stacktrace to obtain the exact name of the property.");
        }
    }

    public static void AssignIfNotSet<Result, IMonoBehaviour>(IMonoBehaviour monoBehaviour, ref Result variable) where IMonoBehaviour : MonoBehaviour
    {
        if (variable == null)
        {
            variable = monoBehaviour.GetComponent<Result>();
        }
        else
        {
            string resultAsString = variable.ToString();
            if (resultAsString == "null")
            {
                variable = monoBehaviour.GetComponent<Result>();
            }
        }
    }
}
