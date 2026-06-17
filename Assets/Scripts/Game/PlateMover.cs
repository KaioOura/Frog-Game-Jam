using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlateMover : MonoBehaviour
{
    public event Action OnRequestFood;
    public event Action OnEndPath;

    // Quantos tiles este prato já percorreu (usado para espaçar o spawn).
    public int TilesTraversed { get; private set; }

    private ConveyorTile _currentTile;
    private Vector3 targetPos;
    private ConveyorManager _conveyorManager;
    private float _speed = 2f;
    
    void Start()
    {
        if (_currentTile != null)
            targetPos = _currentTile.GetExitPoint();
    }

    public void Initialize(ConveyorManager conveyorManager, ConveyorTile startConveyorTile)
    {
        _conveyorManager = conveyorManager;
        _currentTile = startConveyorTile;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
    
    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);

        CheckTiles();
    }

    private void CheckTiles()
    {
        if (!(Vector3.Distance(transform.position, targetPos) < 0.01f)) return;
        
        ConveyorTile next = _conveyorManager.GetNextTile(_currentTile);

        if (next == null)
        {
            _currentTile = null; // saiu da esteira
            return;
        }
        
        switch (next.TileType)
        {
            case TileType.Empty:
                OnEndPath?.Invoke();
                break;
            case TileType.NeedFood:
                OnRequestFood?.Invoke();
                break;
        }
        
        _currentTile = next;
        targetPos = _currentTile.GetExitPoint();
        TilesTraversed++;
    }
}