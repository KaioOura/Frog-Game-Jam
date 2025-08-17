using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoodPlate : MonoBehaviour
{
    public Treadmill treadmill;
    public int posIndex;
    public Transform spawnPoint;
    public Transform foodOnPlatePos;

    private bool Shaken;
    private Ingredient _currentIngredient;

    [SerializeField]
    private Animator plateAnimator;
    private float shakeCount;


    // Start is called before the first frame update
    void Start()
    {
        plateAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //ShakePlate();
        if (Vector3.Distance(transform.position, treadmill.points[posIndex].position) < treadmill.speed * Time.deltaTime) //Checa se a posi��o do prato chegou no atual ponto da esteira
        {
            posIndex++;
        }
        
        if (posIndex > treadmill.points.Length - 1) //Chegou no fim da esteira ALSO se for prato pronto creditar pontos
        {
            posIndex = 0;
            transform.position = spawnPoint.position;
            RemoveIngredient();
        }
        
        transform.position = Vector3.MoveTowards(transform.position, treadmill.points[posIndex].position, treadmill.speed * Time.deltaTime); //Move os pratos
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
