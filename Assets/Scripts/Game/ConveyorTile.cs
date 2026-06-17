using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum Dir
{
    Up,
    Down,
    Left,
    Right
}

public enum TileType
{
    Start,
    Standard,
    NeedFood,
    Empty
}

public class ConveyorTile : MonoBehaviour
{
    public Transform Target => target;
    public Dir OutDirection => outDir;
    public Vector2 GridPos => gridPos;
    public TileType TileType => tileType;
    public MeshRenderer TileRender => tileRender;

    [SerializeField] private Dir outDir; // direção da esteira
    [SerializeField] private Vector2 gridPos; // posição no grid (definida na cena)
    [SerializeField] private Transform target;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private bool isEditing;
    [SerializeField] private TileType tileType;

    [SerializeField] private int[] flowRotations;
    [SerializeField] private MeshRenderer tileRender;

    private void OnValidate()
    {
        Vector3 zeroPos = new Vector2(transform.localPosition.x, transform.localPosition.z) ;
        gridPos = RoundToOneDecimal(zeroPos);
        
        visualTransform.localEulerAngles = new Vector3(0,flowRotations[(int)outDir], 0) ;
        
        target.gameObject.SetActive(isEditing);
    }

    Vector2 RoundToOneDecimal(Vector2 v)
    {
        return new Vector2(
            Mathf.Round(v.x * 10f) / 10f,
            Mathf.Round(v.y * 10f) / 10f
        );
    }
    
    public void Start()
    {
        isEditing = false;
    }

    public Vector3 GetExitPoint()
    {
        return target.position;
    }

#if UNITY_EDITOR
    // Usado pelo ConveyorLayoutBuilder para configurar o tile já posicionado.
    public void EditorConfigure(Dir direction, TileType type)
    {
        outDir = direction;
        tileType = type;
        gridPos = RoundToOneDecimal(new Vector2(transform.localPosition.x, transform.localPosition.z));

        if (visualTransform != null && flowRotations != null && flowRotations.Length > (int)direction)
            visualTransform.localEulerAngles = new Vector3(0, flowRotations[(int)direction], 0);

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}