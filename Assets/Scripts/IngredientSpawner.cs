using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public float urgentBaseWeight = 7f;  // peso do ingrediente urgente quando o tempo está cheio
    public float urgentMaxWeight = 10f; // peso do ingrediente urgente quando o tempo está no final
    public List<IngredientSo> closeExpireIngredients;
    public Treadmill treadmill;
    public IngredientSo rottenIngredient;
    public float timeSpawn = 0.5f;
    
    private OrderManager _orderManager;
    private Order orderCloseToExpire;
    private IEnumerator spawnRoutine;
    float timeTrack;

    public void Initialize(OrderManager orderManager)
    {
        _orderManager = orderManager;
        _orderManager.OnRemoveOrder += OnRemoveIngredientsFromUrgent;
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
        yield return new WaitUntil(() => _orderManager.activeOrders.Count > 0);

        while (true)
        {
            yield return new WaitForSeconds(timeSpawn);
            SpawnIngredient();
        }
    }

    private void SpawnIngredient()
    {
        TreadmillSpot spot = GetFirstFreeSpot();
        if (spot == null) return;

        orderCloseToExpire = FindOrderCloseToExpire();

        IngredientSo ingredientToSpawn = SelectIngredientToSpawn();

        Ingredient instance = Instantiate(ingredientToSpawn.ingredientPrefab, spot.foodOnPlatePos.position, Quaternion.identity);
        spot.AssignIngredient(instance.gameObject);
    }
    
    private TreadmillSpot GetFirstFreeSpot()
    {
        foreach (var spot in treadmill.spots)
        {
            if (spot.posIndex == 0 && !spot.IsOccupied())
                return spot;
        }
        return null;
    }
    
    private Order FindOrderCloseToExpire()
    {
        return _orderManager.activeOrders
            .Where(o => o.IsCloseToExpire())
            .OrderBy(o => o.GetTimeRemainingNormalized())
            .FirstOrDefault();
    }
    
    private IngredientSo SelectIngredientToSpawn()
    {
       Dictionary<IngredientSo, int> weightedIngredients = new Dictionary<IngredientSo, int>();

        // Ingrediente urgente com peso adaptativo
        if (orderCloseToExpire != null)
        {
            float timeRemaining = orderCloseToExpire.GetTimeRemainingNormalized();
            int urgentWeight = (int)Mathf.Lerp(urgentMaxWeight, urgentBaseWeight, timeRemaining);
            
            weightedIngredients.Add(GetUrgentIngredient(), urgentWeight);
        }

        // Ingredientes normais
        foreach (var order in _orderManager.activeOrders)
        {
            if (order.IsCloseToExpire()) continue;

            foreach (IngredientSo ingredientSo in order.myMealSo.recipeIngredientsSo)
            {
                if (!weightedIngredients.ContainsKey(ingredientSo))
                    weightedIngredients.Add(ingredientSo, ingredientSo.baseWeight);
            }
           
        }
        weightedIngredients.Add(rottenIngredient, rottenIngredient.baseWeight);
        
        return ChooseWeightedRandom(weightedIngredients);
    }

    private IngredientSo ChooseWeightedRandom(Dictionary<IngredientSo, int> weightedList)
    {
        float totalWeight = 0f;
        foreach (var item in weightedList)
            totalWeight += item.Value;

        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0f;

        foreach (var item in weightedList)
        {
            currentSum += item.Value;
            if (randomValue <= currentSum)
                return item.Key;
        }

        return weightedList.Keys.Last();
    }
    
    private IngredientSo GetUrgentIngredient()
    {
        // Lista de ingredientes do pedido
        var recipeIngredients = orderCloseToExpire.myMealSo.recipeIngredientsSo
            .Select(r => r)
            .ToList();

        // Procura o primeiro ingrediente que ainda não foi spawnado
        var nextIngredient = recipeIngredients
            .FirstOrDefault(ing => !closeExpireIngredients.Contains(ing));

        // Se todos já foram spawnados, reinicia o ciclo pegando o primeiro
        if (nextIngredient == null)
        {
            closeExpireIngredients.Clear();
            nextIngredient = recipeIngredients.First();
        }

        // Marca que este ingrediente já foi escolhido
        closeExpireIngredients.Add(nextIngredient);

        return nextIngredient;
    }

    private void OnRemoveIngredientsFromUrgent(IngredientSo[] ingredients)
    {
        closeExpireIngredients.RemoveAll(ingredients.Contains);
    }
}