using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : Singleton<GameEventManager>
{
    private Dictionary<string, Action<object>> eventDictionary = new Dictionary<string, Action<object>>();

    public void StartListening(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent += listener;
            eventDictionary[eventName] = thisEvent;
        }
        else
        {
            eventDictionary.Add(eventName, listener);
        }
    }

    public void StopListening(string eventName, Action<object> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent -= listener;
            if (thisEvent == null)
                eventDictionary.Remove(eventName);
            else
                eventDictionary[eventName] = thisEvent;
        }
    }

    public void TriggerEvent(string eventName, object param = null)
    {
        if (eventDictionary.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }
}


public class EventName
{
    public static string OnDataUpdated = "OnDataUpdated";
    public static string ColorWaitUpdate = "ColorWaitUpdate";
    public static string UpdateScore = "UpdateScore";
}