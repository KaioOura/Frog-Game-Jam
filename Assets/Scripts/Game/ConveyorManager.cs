using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public List<PlateMover> PlateMover => plateMovers;
    public List<FoodPlate> FoodPlates => _foodPlates;
    
    [SerializeField] private List<PlateMover> plateMovers = new List<PlateMover>();
    [SerializeField] private IngredientSpawner ingredientSpawner;
    [SerializeField] private List<float> speedList = new List<float>();
    [SerializeField] private List<float> tileSpeedList = new List<float>();
    [SerializeField] private Material sharedTreadMillMaterial;
    
    private List<FoodPlate> _foodPlates = new List<FoodPlate>(); 
    
    private Dictionary<Vector2, ConveyorTile> tiles = new();

    [ContextMenu("Get All PlateMovers")]
    public void GetAllPlateMovers()
    {
        plateMovers = FindObjectsByType<PlateMover>((FindObjectsSortMode)FindObjectsInactive.Exclude).ToList();
    }
    
    void Awake()
    {
        tiles.Clear();

        // registra todas as tiles na cena
        foreach (var tile in FindObjectsByType<ConveyorTile>((FindObjectsSortMode)FindObjectsInactive.Exclude))
        {
            if (!tiles.ContainsKey(tile.GridPos))
                tiles.Add(tile.GridPos, tile);
        }

        foreach (var plate in plateMovers)
        {
            plate.Initialize(this);
            FoodPlate foodPlate = plate.GetComponent<FoodPlate>();
            foodPlate.Initialize(ingredientSpawner);
            
            _foodPlates.Add(foodPlate);
        }
    }

    public void Initialize(DifficultyManager difficultyManager)
    {
        difficultyManager.OnChangeDifficulty += ChangeSpeed;
    }
    
    private void ChangeSpeed(Difficulty difficulty)
    {
        for (int i = 0; i < plateMovers.Count; i++)
        {
            plateMovers[i].ChangeSpeed(speedList[(int)difficulty], tileSpeedList[(int)difficulty]);
        }
    }
    
    public void FixedUpdate()
    {
        for (int i = 0; i < plateMovers.Count; i++)
        {
            plateMovers[i].Move();
        }
    }
    
    public ConveyorTile GetNextTile(ConveyorTile current)
    {
        Vector2Int offset = current.OutDirection switch
        {
            Dir.Up    => new Vector2Int(0, 1),
            Dir.Down  => new Vector2Int(0, -1),
            Dir.Right => new Vector2Int(1, 0),
            Dir.Left  => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };

        Vector2 nextPos = current.GridPos + offset;
        tiles.TryGetValue(nextPos, out ConveyorTile next);
        return next;
    }
}