using System;
using UnityEngine;

public class PlateMover : MonoBehaviour
{
    public event Action OnRequestFood;
    public event Action OnEndPath;
    
    public float speed = 2f;
    public ConveyorTile currentTile;
    private Vector3 targetPos;
    private ConveyorManager _conveyorManager;
    
    void Start()
    {
        if (currentTile != null)
            targetPos = currentTile.GetExitPoint();
    }

    public void Initialize(ConveyorManager conveyorManager)
    {
        _conveyorManager = conveyorManager;
    }

    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

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
    
    void FixedUpdate()
    {
        // move para o alvo
        

        // chegou no fim da tile
        
    }
}