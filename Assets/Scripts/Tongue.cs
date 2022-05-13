using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    public BellyFrog bellyFrog;
    public Animator an;
    public Transform ingredientPos;
    public bool isTongueOccupied;
    public IngredientScriptable ingredientCollected;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AttackTongue()
    {
        an.SetTrigger("Attack");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable") && !isTongueOccupied && !bellyFrog.IsBellyFull())
        {
            OnTongueHit(other.GetComponent<IngredientScriptable>());
            Debug.Log("Licked: " + other.gameObject);
        }
    }

    public void OnTongueHit(IngredientScriptable ingredient)
    {
        isTongueOccupied = true;
        an.SetTrigger("PickedSomething");

        //Colocar ingrediente na lingua

        ingredientCollected = ingredient;
        ingredientCollected.OnCollected();
        ingredientCollected.transform.SetParent(ingredientPos);
        ingredientCollected.transform.localPosition = Vector3.zero;

    }

    public void OnTongueRecover()
    {
        isTongueOccupied = false;
        //Adicionar ingrediente na lista de ingredientes na barriga
        if (ingredientCollected != null)
        {
            bellyFrog.AddIngredient(ingredientCollected);
            ingredientCollected = null;
        }

    }
}
