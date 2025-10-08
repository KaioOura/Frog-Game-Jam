using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlateMover : MonoBehaviour
{
    private static readonly int Speed = Shader.PropertyToID("_Speed");
    public event Action OnRequestFood;
    public event Action OnEndPath;
    
    public ConveyorTile currentTile;
    private Vector3 targetPos;
    private ConveyorManager _conveyorManager;
    private float _speed = 2f;
    
    void Start()
    {
        if (currentTile != null)
            targetPos = currentTile.GetExitPoint();
    }

    public void Initialize(ConveyorManager conveyorManager)
    {
        _conveyorManager = conveyorManager;
    }

    public void ChangeSpeed(float speed, float tileSpeed)
    {
        _speed = speed;
        currentTile.TileRender.material.SetFloat(Speed, tileSpeed);
    }
    
    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);

        CheckTiles();
    }

    private void CheckTiles()
    {
        if (!(Vector3.Distance(transform.position, targetPos) < 0.01f)) return;
        
        ConveyorTile next = _conveyorManager.GetNextTile(currentTile);

        if (next == null)
        {
            currentTile = null; // saiu da esteira
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
        
        currentTile = next;
        targetPos = currentTile.GetExitPoint();
    }
}