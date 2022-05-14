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



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, treadmill.treadMillPoints[posIndex].position) < treadmill.speed * Time.deltaTime) //Checa se a posi��o do prato chegou no atual ponto da esteira
        {
            posIndex++;
        }

        if (posIndex > treadmill.treadMillPoints.Length - 1) //Chegou no fim da esteira ALSO se for prato pronto creditar pontos
        {
            posIndex = 0;
            transform.position = spawnPoint.position;
            DestroyIngredient();

        }

        transform.position = Vector3.MoveTowards(transform.position, treadmill.treadMillPoints[posIndex].position, treadmill.speed * Time.deltaTime); //Move os pratos
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

    }
}
