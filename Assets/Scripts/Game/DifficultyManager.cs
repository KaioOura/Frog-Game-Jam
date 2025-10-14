using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public event Action<Difficulty> OnChangeDifficulty;
    
    [SerializeField] private Difficulty difficulty;
    
    [Tooltip("Time in seconds to change difficulty")]
    [SerializeField] private int[] difficultyBreakdown;
    
    private int _difficultyIndex;
    
    private float _timeTracker;
    
    
    [Header("Debug")]
    [SerializeField] private Difficulty difficultyDebug;

    [ContextMenu("Change Difficulty")]
    public void ChangeDifficultyDebug()
    {
        ChangeDifficulty(difficultyDebug);
    }
    
    // Update is called once per frame
    void Update()
    {
        _timeTracker += Time.deltaTime;
        
        if (_timeTracker > difficultyBreakdown[_difficultyIndex] && difficulty is not Difficulty.hard)
        {
            _difficultyIndex++;
            _timeTracker = 0;
            
            ChangeDifficulty((Difficulty)_difficultyIndex);
        }
    }

    public Difficulty GetDifficulty()
    {
        return difficulty;
    }
    
    public void ResetDifficulty()
    {
        _timeTracker = 0;
        ChangeDifficulty(Difficulty.easy);
    }

    private void ChangeDifficulty(Difficulty difficulty)
    {
        this.difficulty = difficulty;
        
        OnChangeDifficulty?.Invoke(this.difficulty);
    }

    public Difficulty GetCurrentDifficulty()
    {
        return difficulty;
    }
}
