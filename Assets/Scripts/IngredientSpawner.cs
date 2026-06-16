using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IngredientSpawner : MonoBehaviour
{
    public float urgentBaseWeight = 7f; // peso do ingrediente urgente quando o tempo está cheio
    public float urgentMaxWeight = 10f; // peso do ingrediente urgente quando o tempo está no final
    public IngredientSo rottenIngredient;
    [SerializeField] private bool spawnOnlyBomb;


    private OrderManager _orderManager;
    private ObjectPoolManager _objectPoolManager;
    private ConveyorManager _conveyorManager;
    private Order orderCloseToExpire;

    private readonly Dictionary<IngredientSo, int> _weightedIngredients = new Dictionary<IngredientSo, int>();

    public void Initialize(OrderManager orderManager, ObjectPoolManager objectPoolManager,
        ConveyorManager conveyorManager)
    {
        _orderManager = orderManager;
        _objectPoolManager = objectPoolManager;
        _conveyorManager = conveyorManager;
    }

    public bool HasOrderActive()
    {
        return _orderManager.ActiveOrders.Count > 0;
    }

    public Ingredient SpawnIngredient()
    {
        orderCloseToExpire = FindOrderCloseToExpire();

        IngredientSo ingredientToSpawn = SelectIngredientToSpawn();
        Ingredient ingredient = _objectPoolManager.IngredientPoolManager.Pool[ingredientToSpawn].Get();

        //Ingredient instance = Instantiate(ingredientToSpawn.ingredientPrefab, spot.foodOnPlatePos.position, Quaternion.identity);

        return ingredient;
    }

    // private FoodPlate GetFirstFreeSpot()
    // {
    //     foreach (var spot in _conveyorManager.FoodPlates)
    //     {
    //         if (spot.PlateMover.CurrentStep <= 0 && !spot.IsOccupied())
    //             return spot;
    //     }
    //     return null;
    // }

    private Order FindOrderCloseToExpire()
    {
        return _orderManager.ActiveOrders
            .Where(o => o.IsCloseToExpire())
            .OrderBy(o => o.GetTimeRemainingNormalized())
            .FirstOrDefault();
    }

    private IngredientSo SelectIngredientToSpawn()
    {
        _weightedIngredients.Clear();

#if UNITY_EDITOR
        if (spawnOnlyBomb)
            return rottenIngredient;
#endif

        // Ingrediente urgente com peso adaptativo
        if (orderCloseToExpire != null)
        {
            float timeRemaining = orderCloseToExpire.GetTimeRemainingNormalized();
            int urgentWeight = (int)Mathf.Lerp(urgentMaxWeight, urgentBaseWeight, timeRemaining);

            _weightedIngredients.Add(orderCloseToExpire.GetNextUrgentIngredient(), urgentWeight);
        }

        // Ingredientes normais
        foreach (var order in _orderManager.ActiveOrders)
        {
            if (order.IsCloseToExpire()) continue;

            foreach (IngredientSo ingredientSo in order.myMealSo.recipeIngredientsSo)
            {
                if (!_weightedIngredients.ContainsKey(ingredientSo))
                    _weightedIngredients.Add(ingredientSo, ingredientSo.baseWeight);
            }
        }

        if (!_weightedIngredients.ContainsKey(rottenIngredient))
            _weightedIngredients.Add(rottenIngredient, rottenIngredient.baseWeight);

        return ChooseWeightedRandom(_weightedIngredients);
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
}