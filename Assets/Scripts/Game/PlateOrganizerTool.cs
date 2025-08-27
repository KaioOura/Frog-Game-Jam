using System.Collections.Generic;
using UnityEngine;

public class PlateOrganizerTool : MonoBehaviour
{
    [Header("Configurações")]
    public List<Transform> objects = new List<Transform>();   // Objetos a posicionar
    public Transform startPosition; // Ponto inicial
    public Vector3 direction = Vector3.right; // Direção de espaçamento
    public float spacing = 1f;    // Distância entre cada objeto
    
    [ContextMenu("Posicionar Objetos")]
    void PositionObjects()
    {
        objects.Clear();
        
        foreach (Transform child in transform)
        {
            objects.Add(child);
        }
        
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null)
            {
                objects[i].position = startPosition.position + direction.normalized * spacing * i;
            }
        }
    }
}
