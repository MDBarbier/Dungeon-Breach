using Assets.Scripts.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderLogHandler : MonoBehaviour
{
    private List<LoggingEntry> turnLogEntries;
    private Text turnLogText;

    // Start is called before the first frame update
    void Start()
    {
        turnLogEntries = new List<LoggingEntry>();
        turnLogText = GetComponent<Text>();
        turnLogText.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void TurnLog(List<Character> turnOrder)
    {
        ClearturnLog();

        var initiativeList = turnOrder.ToList();

        foreach (var item in initiativeList)
        {
            turnLogEntries.Add(new LoggingEntry() { Timestamp = DateTime.Now, Text = $"{item.Name}" });
        }
        
        string text = string.Empty;

        foreach (var logEntry in turnLogEntries)
        {
            text += $"{logEntry.Text}\n";
        }

        turnLogText.text = text;
    }

    internal void ClearturnLog()
    {
        turnLogEntries.Clear();
        turnLogEntries = new List<LoggingEntry>();
    }
}
