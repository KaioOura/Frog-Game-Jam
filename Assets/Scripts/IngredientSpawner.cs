using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public IngredientScriptable[] ingredientsPrefab;
    public List<IngredientScriptable> priorityIngredients; 
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

        if (OrderManager.instance.activeOrders.Count < 1)
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

                foreach (var item in OrderManager.instance.activeOrders)
                {
                    if (!item.isOnPriorityLine && item.timeCount.fillAmount <= 0.4f)
                    {
                        item.isOnPriorityLine = true;

                        foreach (var item2 in item.myMeal.recipeIngredients)
                        {
                            priorityIngredients.Add(item2.ingredientScriptable);
                        }
                    }
                    else
                    {
                        foreach (var item2 in item.myMeal.recipeIngredients)
                        {
                            ingredientsAvailable.Add(item2.ingredientScriptable);
                        }
                    }
                    
                }

                IngredientScriptable go;

                if (priorityIngredients.Count > 0)
                {
                    go = Instantiate(priorityIngredients[0], treadmill.positions[i].foodOnPlatePos.position, Quaternion.identity);
                    priorityIngredients.Remove(priorityIngredients[0]);
                }
                else
                {
                    int rand = Random.Range(0, ingredientsAvailable.Count);
                    go = Instantiate(ingredientsAvailable[rand], treadmill.positions[i].foodOnPlatePos.position, Quaternion.identity);
                }
               
                treadmill.positions[i].AssignIngredient(go.gameObject);
                break;
            }
        }
    }
}
