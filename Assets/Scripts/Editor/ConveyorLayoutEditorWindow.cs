using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChefrogEditor
{
    /// <summary>
    /// Editor visual tile-based para desenhar layouts de esteira.
    /// Botão esquerdo pinta o tile selecionado (clicar de novo rotaciona);
    /// botão direito apaga. "Build Layout" instancia o resultado na cena.
    /// </summary>
    public class ConveyorLayoutEditorWindow : EditorWindow
    {
        // Ordem de rotação (sentido horário).
        private static readonly Dir[] RotationOrder = { Dir.Up, Dir.Right, Dir.Down, Dir.Left };
        private static readonly TileType[] PaletteTypes =
            { TileType.Standard, TileType.Start, TileType.NeedFood, TileType.Empty };

        private const int CellSize = 40;
        private const int MaxGridSize = 64;

        [SerializeField] private int gridWidth = 6;
        [SerializeField] private int gridHeight = 6;
        [SerializeField] private float step = 1f;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Transform parent;
        [SerializeField] private ConveyorManager conveyorManager;
        [SerializeField] private int selectedPaletteIndex;

        // Estado pintado. Não sobrevive a recompilações (aceitável para uma tool).
        private readonly Dictionary<Vector2Int, PaintedTile> _painted = new Dictionary<Vector2Int, PaintedTile>();
        private Vector2 _scroll;

        private struct PaintedTile
        {
            public TileType type;
            public Dir dir;
        }

        [MenuItem("Tools/Chefrog/Conveyor Layout Editor")]
        public static void Open()
        {
            GetWindow<ConveyorLayoutEditorWindow>("Conveyor Layout");
        }

        private void OnGUI()
        {
            DrawGridSize();
            DrawPalette();
            DrawGrid();
            DrawOutputAndActions();
        }

        private void DrawGridSize()
        {
            EditorGUILayout.LabelField("Tamanho da Grid", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                gridWidth = Mathf.Clamp(EditorGUILayout.IntField("Largura", gridWidth), 1, MaxGridSize);
                gridHeight = Mathf.Clamp(EditorGUILayout.IntField("Altura", gridHeight), 1, MaxGridSize);
            }
            EditorGUILayout.Space(6);
        }

        private void DrawPalette()
        {
            EditorGUILayout.LabelField("Paleta", EditorStyles.boldLabel);

            var names = new string[PaletteTypes.Length];
            for (int i = 0; i < PaletteTypes.Length; i++)
                names[i] = PaletteTypes[i].ToString();

            selectedPaletteIndex = GUILayout.Toolbar(selectedPaletteIndex, names);
            EditorGUILayout.HelpBox("Esquerdo: pinta (clique de novo p/ rotacionar). Direito: apaga.", MessageType.None);
            EditorGUILayout.Space(6);
        }

        private void DrawGrid()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));

            Rect area = GUILayoutUtility.GetRect(gridWidth * CellSize, gridHeight * CellSize, GUILayout.ExpandWidth(false));

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    var cell = new Vector2Int(x, y);
                    int screenRow = gridHeight - 1 - y; // y cresce para cima
                    var rect = new Rect(area.x + x * CellSize, area.y + screenRow * CellSize, CellSize - 2, CellSize - 2);

                    DrawCell(rect, cell);
                    HandleCellInput(rect, cell);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCell(Rect rect, Vector2Int cell)
        {
            if (Event.current.type != EventType.Repaint) return;

            Color prev = GUI.backgroundColor;
            string label = string.Empty;

            if (_painted.TryGetValue(cell, out PaintedTile tile))
            {
                GUI.backgroundColor = ColorFor(tile.type);
                label = $"{LetterFor(tile.type)}\n{ArrowFor(tile.dir)}";
            }
            else
            {
                GUI.backgroundColor = new Color(0.25f, 0.25f, 0.25f);
            }

            GUI.Box(rect, label, EditorStyles.helpBox);
            GUI.backgroundColor = prev;
        }

        private void HandleCellInput(Rect rect, Vector2Int cell)
        {
            Event e = Event.current;
            if (e.type != EventType.MouseDown || !rect.Contains(e.mousePosition)) return;

            if (e.button == 0) Paint(cell);
            else if (e.button == 1) _painted.Remove(cell);

            e.Use();
            Repaint();
        }

        private void Paint(Vector2Int cell)
        {
            TileType selected = PaletteTypes[selectedPaletteIndex];

            if (_painted.TryGetValue(cell, out PaintedTile existing) && existing.type == selected)
            {
                existing.dir = NextDir(existing.dir); // mesmo tipo: rotaciona
                _painted[cell] = existing;
                return;
            }

            _painted[cell] = new PaintedTile { type = selected, dir = Dir.Right };
        }

        private void DrawOutputAndActions()
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Saída", EditorStyles.boldLabel);
            tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
            parent = (Transform)EditorGUILayout.ObjectField("Parent (origem)", parent, typeof(Transform), true);
            conveyorManager = (ConveyorManager)EditorGUILayout.ObjectField("Conveyor Manager", conveyorManager, typeof(ConveyorManager), true);
            step = EditorGUILayout.FloatField("Step", step);

            EditorGUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Build Layout", GUILayout.Height(28)))
                    Build();

                if (GUILayout.Button("Clear Grid", GUILayout.Height(28)))
                    _painted.Clear();
            }
        }

        private void Build()
        {
            var specs = new List<ConveyorLayoutBuildService.TileSpec>();
            foreach (var kv in _painted)
            {
                Vector2Int cell = kv.Key;
                if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight) continue;

                specs.Add(new ConveyorLayoutBuildService.TileSpec
                {
                    cell = cell,
                    dir = kv.Value.dir,
                    type = kv.Value.type
                });
            }

            if (ConveyorLayoutBuildService.TryBuild(specs, tilePrefab, parent, step, conveyorManager, out string error))
                Debug.Log($"[ConveyorLayout] Layout gerado com {specs.Count} tiles.");
            else
                EditorUtility.DisplayDialog("Conveyor Layout", error, "OK");
        }

        private static Dir NextDir(Dir current)
        {
            int i = System.Array.IndexOf(RotationOrder, current);
            return RotationOrder[(i + 1) % RotationOrder.Length];
        }

        private static Color ColorFor(TileType type) => type switch
        {
            TileType.Start => new Color(0.40f, 0.75f, 0.40f),
            TileType.NeedFood => new Color(0.90f, 0.60f, 0.30f),
            TileType.Empty => new Color(0.45f, 0.65f, 0.95f),
            _ => new Color(0.75f, 0.75f, 0.75f)
        };

        private static string LetterFor(TileType type) => type switch
        {
            TileType.Start => "S",
            TileType.NeedFood => "F",
            TileType.Empty => "E",
            _ => ""
        };

        private static string ArrowFor(Dir dir) => dir switch
        {
            Dir.Up => "↑",
            Dir.Down => "↓",
            Dir.Left => "←",
            _ => "→"
        };
    }
}
