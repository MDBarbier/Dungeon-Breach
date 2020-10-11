using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatLogHandler : MonoBehaviour
{

    private List<LoggingEntry> combatLogEntries;
    private Text combatLogText;

    // Start is called before the first frame update
    void Start()
    {
        combatLogEntries = new List<LoggingEntry>();
        combatLogText = GetComponent<Text>();
        combatLogText.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void CombatLog(string message)
    {
        if (combatLogEntries.Count >= 10)
        {
            ClearCombatLog();
        }

        combatLogEntries.Add(new LoggingEntry() { Timestamp = DateTime.Now, Text = message });
        string text = string.Empty;

        foreach (var logEntry in combatLogEntries)
        {
            text += $"{logEntry.Timestamp.ToShortTimeString().ToString()}: {logEntry.Text}\n";
        }

        combatLogText.text = text;
    }

    internal void ClearCombatLog()
    {
        combatLogEntries.Clear();
        combatLogEntries = new List<LoggingEntry>();
    }
}
