using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoodPlate : MonoBehaviour
{
    public PlateMover PlateMover => _plateMover;
    
    public Treadmill treadmill;
    public int posIndex;
    public Transform spawnPoint;
    public Transform foodOnPlatePos;

    private bool Shaken;
    private Ingredient _currentIngredient;
    private PlateMover _plateMover;
    private IngredientSpawner _ingredientSpawner;

    [SerializeField]
    private Animator plateAnimator;
    private float shakeCount;


    // Start is called before the first frame update
    void Start()
    {
        plateAnimator = GetComponentInChildren<Animator>();

    }

    public void Initialize(IngredientSpawner ingredientSpawner)
    {
        _ingredientSpawner = ingredientSpawner;

        _plateMover = GetComponent<PlateMover>();
        
        _plateMover.OnRequestFood += OnFoodRequested;
        _plateMover.OnEndPath += RemoveIngredient;
    }

    public void OnFoodRequested()
    {
        AssignIngredient(_ingredientSpawner.SpawnIngredient());
    }
    
    public void AssignIngredient(Ingredient ingredient)
    {
        _currentIngredient = ingredient;
        
        _currentIngredient.transform.parent = transform;
        _currentIngredient.transform.localPosition = Vector3.zero;
        
        _currentIngredient.OnGetRemovedFromPlate += OnIngredientCollected;
    }

    public void RemoveIngredient()
    {
        if (_currentIngredient == null) return;
        
        _currentIngredient.ReleaseToPool();
        _currentIngredient = null;
    }

    private void OnIngredientCollected()
    {
        _currentIngredient = null;
    }
    
    public bool IsOccupied()
    {
        return _currentIngredient != null;
    }

    public void ShakePlate(){
        if(foodOnPlatePos.childCount != 0){
            Shaken = false;
            shakeCount = 0;
        }else{
            Shaken = true;
        }
        if(Shaken && shakeCount == 1){
            plateAnimator.SetTrigger("ShakePlate");
            Shaken = false;
            shakeCount++;
        }else if(shakeCount == 0){
            shakeCount++;
        }
    }
}
