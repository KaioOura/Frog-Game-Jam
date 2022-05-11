using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public GameObject tomatoPrefab;
    public Treadmill treadmill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnIngredient();
        }
    }

    void SpawnIngredient()
    {
        for (int i = 0; i < treadmill.positions.Count; i++)
        {
            //Checa se existe um prato que ainda não ativo e se não está ocupado
            if (treadmill.positions[i].posIndex == 0 && !treadmill.positions[i].IsOccupied())
            {
                GameObject go = Instantiate(tomatoPrefab, treadmill.positions[i].foodOnPlatePos.position, Quaternion.identity);
                treadmill.positions[i].AssignIngredient(go);
                break;
            }
        }
    }
}
