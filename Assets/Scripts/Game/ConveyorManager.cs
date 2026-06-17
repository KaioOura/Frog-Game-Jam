using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public List<PlateMover> PlateMover => plateMovers;
    public List<FoodPlate> FoodPlates => _foodPlates;
    
    [SerializeField] private List<ConveyorTile> tilesList = new List<ConveyorTile>();
    [SerializeField] private IngredientSpawner ingredientSpawner;
    [SerializeField] private ConveyorTile startTile;
    [SerializeField] private List<float> speedList = new List<float>();
    [SerializeField] private List<float> tileSpeedList = new List<float>();
    [SerializeField] private Material sharedTreadMillMaterial;

    [Header("Pratos (gerados automaticamente)")]
    [SerializeField] private PlateMover platePrefab;
    [Tooltip("Espaçamento-alvo entre pratos, em tiles.")]
    [SerializeField] private int plateSpacing = 3;
    [Tooltip("Pai dos pratos gerados. Se vazio, usa este transform.")]
    [SerializeField] private Transform platesParent;

    // Preenchida em runtime por SpawnAndDistributePlates.
    private List<PlateMover> plateMovers = new List<PlateMover>();
    
    private List<FoodPlate> _foodPlates = new List<FoodPlate>();

    private Dictionary<Vector2, ConveyorTile> tiles = new();

    private static readonly int SpeedId = Shader.PropertyToID("_Speed");
    private DifficultyManager _difficultyManager;

#if UNITY_EDITOR
    // Usado pelo editor de layout para apontar a esteira recém-gerada.
    public void EditorSetLayout(List<ConveyorTile> tiles, ConveyorTile start)
    {
        tilesList = tiles;
        startTile = start;
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    void Awake()
    {
        tiles.Clear();

        // registra todas as tiles na cena
        foreach (var tile in tilesList)
        {
            tiles.TryAdd(tile.GridPos, tile);
        }

        SpawnAndDistributePlates();
    }

    // Percorre o caminho ordenado a partir do startTile. A guarda de visitados
    // fecha em loop ou para no fim de um caminho aberto.
    private List<ConveyorTile> BuildPath()
    {
        var path = new List<ConveyorTile>();
        var visited = new HashSet<ConveyorTile>();
        ConveyorTile current = startTile;

        while (current != null && visited.Add(current))
        {
            path.Add(current);
            current = GetNextTile(current);
        }

        return path;
    }

    // Instancia todos os pratos de uma vez, distribuídos uniformemente ao longo do
    // caminho da esteira e encaixados sobre os tiles (âncora = Target do tile).
    private void SpawnAndDistributePlates()
    {
        plateMovers.Clear();
        _foodPlates.Clear();

        if (platePrefab == null)
        {
            Debug.LogError("ConveyorManager: platePrefab não atribuído.");
            return;
        }

        List<ConveyorTile> path = BuildPath();
        if (path.Count == 0)
        {
            Debug.LogError("ConveyorManager: caminho vazio (startTile não definido?).");
            return;
        }

        Transform parent = platesParent != null ? platesParent : transform;
        int gap = Mathf.Max(1, plateSpacing);
        int count = Mathf.Max(1, Mathf.RoundToInt(path.Count / (float)gap));

        for (int i = 0; i < count; i++)
        {
            int idx = Mathf.RoundToInt(i * path.Count / (float)count) % path.Count;
            ConveyorTile tile = path[idx];

            Vector3 pos = tile.Target != null ? tile.Target.position : tile.transform.position;
            PlateMover plate = Instantiate(platePrefab, pos, Quaternion.identity, parent);
            plate.Initialize(this, tile);

            FoodPlate foodPlate = plate.GetComponent<FoodPlate>();
            foodPlate.Initialize(ingredientSpawner);

            if (_difficultyManager != null)
                plate.SetSpeed(speedList[(int)_difficultyManager.GetDifficulty()]);

            plateMovers.Add(plate);
            _foodPlates.Add(foodPlate);
        }
    }

    public void Initialize(DifficultyManager difficultyManager)
    {
        _difficultyManager = difficultyManager;
        difficultyManager.OnChangeDifficulty += ChangeSpeed;
    }

    private void ChangeSpeed(Difficulty difficulty)
    {
        // A esteira inteira rola de forma uniforme pelo material compartilhado.
        if (sharedTreadMillMaterial != null)
            sharedTreadMillMaterial.SetFloat(SpeedId, tileSpeedList[(int)difficulty]);

        float plateSpeed = speedList[(int)difficulty];
        for (int i = 0; i < plateMovers.Count; i++)
            plateMovers[i].SetSpeed(plateSpeed);
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