using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private LeaderboardEntryUI leaderboardEntryUIPrefab;
    [SerializeField] private Transform container;
    
    private List<LeaderboardEntryUI> _leaderboardEntryUI = new List<LeaderboardEntryUI>();
    private LeaderboardManager _leaderboardManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            LeaderboardEntryUI leaderboardEntryUI = Instantiate(leaderboardEntryUIPrefab, container);
            _leaderboardEntryUI.Add(leaderboardEntryUI);
        }
    }

    public void InitializeLeaderboard(LeaderboardManager leaderboardManager)
    {
        _leaderboardManager = leaderboardManager;

        _leaderboardManager.OnLeaderboardUpdated += UpdateUI;
        
        RequestLeaderboardList();
    }

    private void OnEnable()
    {
        RequestLeaderboardList();
    }

    private void RequestLeaderboardList()
    {
        _leaderboardManager.GetEntries();
    }
    
    private void UpdateUI(List<LeaderboardEntry> leaderboardEntries)
    {
        for (var index = 0; index < _leaderboardEntryUI.Count; index++)
        {
            var entryUI = _leaderboardEntryUI[index];
            
            entryUI.Initialize(leaderboardEntries[index], index);
        }
    }
}
