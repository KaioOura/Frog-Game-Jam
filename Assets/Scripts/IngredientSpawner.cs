using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public IngredientScriptable[] ingredientsPrefab;
    public List<IngredientScriptable> priorityIngredients;
    public List<IngredientScriptable> closeExpireIngredients;
    public Treadmill treadmill;
    public IngredientScriptable rottenIngredient;
    private Order orderCloseToExpire;
    private IEnumerator spawnRoutine;

    public int chanceToSpawnRotten;

    public float timeSpawn = 0.5f;
    float timeTrack;

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

    public void StartIngredientSpawn()
    {
        spawnRoutine = SpawnIngredientRoutine();
        StartCoroutine(spawnRoutine);
    }

    public void StopIngredientSpawn()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    IEnumerator SpawnIngredientRoutine()
    {
        yield return new WaitUntil(() => OrderManager.instance.activeOrders.Count > 0);

        while (true)
        {
            yield return new WaitForSeconds(timeSpawn);
            SpawnIngredient();
        }
    }

    void SpawnIngredient()
    {
        for (int i = 0; i < treadmill.positions.Count; i++)
        {
            //Checa se existe um prato que ainda n�o ativo e se n�o est� ocupado
            if (treadmill.positions[i].posIndex == 0 && !treadmill.positions[i].IsOccupied())
            {
                List<IngredientScriptable> ingredientsAvailable = new List<IngredientScriptable>();

                foreach (var item in OrderManager.instance.activeOrders)
                {
                    if (item.timeCount.fillAmount <= 0.9f && orderCloseToExpire == null)
                    {
                        orderCloseToExpire = item;
                        closeExpireIngredients.Clear();
                    }
                    else
                    {
                        foreach (var item2 in item.myMeal.recipeIngredients)
                            ingredientsAvailable.Add(item2.ingredientScriptable);
                    }
                }

                int randRot = Random.Range(0, 10);
                if (randRot >= 2)
                    ingredientsAvailable.Add(rottenIngredient);

                IngredientScriptable go;

                int randExpireOrderSpawn = Random.Range(0, 10);

                if (randExpireOrderSpawn >= 4 && orderCloseToExpire != null)
                {
                    go = Instantiate(ForceIngredientOrderExpire(), treadmill.positions[i].foodOnPlatePos.position,
                        Quaternion.identity);
                }
                else
                {
                    int rand = Random.Range(0, ingredientsAvailable.Count);
                    go = Instantiate(ingredientsAvailable[rand], treadmill.positions[i].foodOnPlatePos.position,
                        Quaternion.identity);
                }

                treadmill.positions[i].AssignIngredient(go.gameObject);
                break;
            }
        }
    }

    IngredientScriptable ForceIngredientOrderExpire()
    {
        int randIngridient;
        IngredientScriptable ingredientScriptable = null;

        for (int i = 0; i < orderCloseToExpire.myMeal.recipeIngredients.Length; i++)
        {
            if (!closeExpireIngredients.Contains(orderCloseToExpire.myMeal.recipeIngredients[i].ingredientScriptable))
            {
                ingredientScriptable = orderCloseToExpire.myMeal.recipeIngredients[i].ingredientScriptable;
                closeExpireIngredients.Add(ingredientScriptable);
                break;
            }
        }

        if (ingredientScriptable == null)
        {
            closeExpireIngredients.Clear();
            ingredientScriptable = orderCloseToExpire.myMeal.recipeIngredients[0].ingredientScriptable;

            closeExpireIngredients.Add(ingredientScriptable);
        }

        return ingredientScriptable;
    }
}