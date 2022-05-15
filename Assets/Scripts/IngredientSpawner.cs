using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public IngredientScriptable[] ingredientsPrefab;
    public Treadmill treadmill;

    public float timeSpawn = 0.5f;
    float timeTrack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates != GameManager.GameStates.game)
            return;

        if (Time.time > timeTrack)
        {
            timeTrack = Time.time + timeSpawn;
            SpawnIngredient();
        }

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

                List<IngredientScriptable> ingredientsAvailable = new List<IngredientScriptable>();

                for (int y = 0; y < OrderManager.instance.difficultyIndex + 1; y++)
                {
                    if (y == 0)
                    {
                        foreach (var item in ingredientsPrefab)
                        {
                            if (item.ingredient.difficulty == IngredientBase.Difficulty.easy)
                            {
                                ingredientsAvailable.Add(item);
                            }
                        }
                    }
                    else if (y == 1)
                    {
                        foreach (var item in ingredientsPrefab)
                        {
                            if (item.ingredient.difficulty == IngredientBase.Difficulty.normal)
                            {
                                ingredientsAvailable.Add(item);
                            }
                        }
                    }
                    else if (y == 2)
                    {
                        foreach (var item in ingredientsPrefab)
                        {
                            if (item.ingredient.difficulty == IngredientBase.Difficulty.hard)
                            {
                                ingredientsAvailable.Add(item);
                            }
                        }
                    }
                }

                int rand = Random.Range(0, ingredientsAvailable.Count); 

                IngredientScriptable go = Instantiate(ingredientsAvailable[rand], treadmill.positions[i].foodOnPlatePos.position, Quaternion.identity);
                treadmill.positions[i].AssignIngredient(go.gameObject);
                break;
            }
        }
    }
}
