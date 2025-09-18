using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionDataBase : MonoBehaviour
{
    private Dictionary<string, int> _storedActions = new Dictionary<string, int>();

    public void ProcessAction(InGameAction inGameAction) // The progress will need to be saved for future implementations such as missions and achievements. Right now there is no need for saving tutorial progress
    {
        string key = inGameAction.Key;

        if (_storedActions.TryGetValue(key, out int value))
            _storedActions[key] = value + inGameAction.Amount;
        else
            _storedActions.Add(key, inGameAction.Amount);
    }

    public int GetActionAmount(string key)
    {
        return _storedActions.GetValueOrDefault(key, 0);
    }
}
