using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TreadmillPos : MonoBehaviour
{
    public Treadmill treadmill;
    public int posIndex;
    public Transform spawnPoint;
    public Transform foodOnPlatePos;

    private bool Shaken;
    public GameObject plateGO;

    [SerializeField]
    private Animator plateAnimator;
    private float shakeCount;


    // Start is called before the first frame update
    void Start()
    {
        plateAnimator = GetComponentInChildren<Animator>();
        plateGO = plateAnimator.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        // ShakePlate();
        // if (Vector3.Distance(transform.position, treadmill.treadMillPoints[posIndex].position) < treadmill.speed * Time.deltaTime) //Checa se a posi��o do prato chegou no atual ponto da esteira
        // {
        //     posIndex++;
        // }
        //
        // if (posIndex > treadmill.treadMillPoints.Length - 1) //Chegou no fim da esteira ALSO se for prato pronto creditar pontos
        // {
        //     posIndex = 0;
        //     transform.position = spawnPoint.position;
        //     DestroyIngredient();
        //
        // }
        //
        // transform.position = Vector3.MoveTowards(transform.position, treadmill.treadMillPoints[posIndex].position, treadmill.speed * Time.deltaTime); //Move os pratos
    }

    public void AssignIngredient(GameObject ingredient)
    {
        ingredient.transform.SetParent(foodOnPlatePos);
        ingredient.transform.localPosition = Vector3.zero;
    }

    void DestroyIngredient()
    {
        if (foodOnPlatePos.childCount > 0)
            Destroy(foodOnPlatePos.GetChild(0).gameObject);
    }

    public bool IsOccupied()
    {
        return foodOnPlatePos.childCount > 0;
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
