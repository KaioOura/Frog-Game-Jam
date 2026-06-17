using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ChefrogEditor
{
    /// <summary>
    /// Lógica de construção do layout de esteira no mundo, separada da UI.
    /// Recebe a descrição dos tiles (célula + direção + tipo) e instancia o prefab,
    /// configurando cada tile e apontando o resultado no ConveyorManager.
    /// </summary>
    public static class ConveyorLayoutBuildService
    {
        public struct TileSpec
        {
            public Vector2Int cell;
            public Dir dir;
            public TileType type;
        }

        public const string GeneratedRootName = "GeneratedLayout";

        public static bool TryBuild(
            IReadOnlyList<TileSpec> specs,
            GameObject tilePrefab,
            Transform parent,
            float step,
            ConveyorManager conveyorManager,
            out string error)
        {
            if (tilePrefab == null) { error = "Tile prefab não atribuído."; return false; }
            if (tilePrefab.GetComponent<ConveyorTile>() == null) { error = "O prefab não tem o componente ConveyorTile."; return false; }
            if (specs == null || specs.Count == 0) { error = "Nenhum tile pintado na grid."; return false; }

            int startCount = 0;
            foreach (var s in specs)
                if (s.type == TileType.Start) startCount++;

            if (startCount == 0) { error = "O layout precisa de pelo menos um tile Start."; return false; }

            Transform root = CreateRoot(parent);
            Undo.RegisterCreatedObjectUndo(root.gameObject, "Build Conveyor Layout");

            var tiles = new List<ConveyorTile>(specs.Count);
            ConveyorTile startTile = null;

            foreach (var spec in specs)
            {
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, root);
                go.name = $"Tile_{spec.cell.x}_{spec.cell.y}";
                go.transform.localPosition = new Vector3(spec.cell.x * step, 0f, spec.cell.y * step);

                ConveyorTile tile = go.GetComponent<ConveyorTile>();
                tile.EditorConfigure(spec.dir, spec.type);
                tiles.Add(tile);

                if (spec.type == TileType.Start && startTile == null)
                    startTile = tile;
            }

            if (conveyorManager != null)
            {
                Undo.RecordObject(conveyorManager, "Build Conveyor Layout");
                conveyorManager.EditorSetLayout(tiles, startTile);
            }

            EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
            error = null;
            return true;
        }

        private static Transform CreateRoot(Transform parent)
        {
            var root = new GameObject(GeneratedRootName).transform;
            root.SetParent(parent, false);
            return root;
        }
    }
}
