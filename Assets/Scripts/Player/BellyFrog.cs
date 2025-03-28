using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Profiling;

public class BellyFrog : MonoBehaviour
{
    public GameObject saliva_VFX, Jaw_Pos;
    public ParticleSystem Sweat_VFX;

    public bool targeting;
    private IngredientScriptable ingredient;

    public Animation_Controller animationController;
    public Animator CartAnimator;
    public Animator frogController;
    public BellyDisplay bellyDisplay;
    public List<IngredientScriptable> belly;
    public int maxIngredients;
    public Transform bellyPos, JawPos;
    public Tongue tongue;
    public List<Meal> meals;

    public Meal activeMeal;
    GameObject mealGO;

    public float maxTimeInBelly;
    public float timeFoodInBelly;
    public float reduceTimeInBelly;

    public Action OnIngredientAdded;

    bool isThrowingUp;

    public Transform cartPos;

    public AudioSource audioSource;
    public AudioClip[] swallowClip;
    public AudioClip succesMeal;
    public AudioClip hurtClip;


    public LayerMask IngredientLayer;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animation_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates != GameStates.game)
            return;
        CheckFoodInBelly();
    }

    public void AddIngredient(IngredientScriptable ingredient)
    {
        if (belly.Count - 1 >= maxIngredients)
            return;

        belly.Add(ingredient);
        animationController.realayerWeight += 0.25f;
        ingredient.gameObject.SetActive(false);
        ingredient.transform.SetParent(bellyPos.transform);
        ingredient.transform.localPosition = Vector3.zero;

        if (ingredient.isRottenFood)
        {
            //Perder vida, cuspir tudo
            GameManager.instance.ChangeLife(-1);
            ThrowUpAllIngredients();
            return;
        }

        bellyDisplay.UpdateUI();
        OnIngredientAdd();
    }

    public void ThrowUpAllIngredients()
    {
        if (isThrowingUp || tongue.isTongueOccupied)
            return;

        StartCoroutine(ThrowUpIngredients());
    }

    public void MoveMealToCart(Vector3 PosOffset)
    {
        int rand = UnityEngine.Random.Range(0, swallowClip.Length);
        audioSource.PlayOneShot(swallowClip[rand]);
        Instantiate(saliva_VFX, Jaw_Pos.transform.position, gameObject.transform.rotation);
        OrderManager.instance.CheckMeal(activeMeal, true, () => audioSource.PlayOneShot(succesMeal));
        mealGO.SetActive(true);
        mealGO.gameObject.transform.position = JawPos.position + PosOffset;
        mealGO.transform.DOMove(cartPos.position, 0.1f).OnComplete(() =>
        {
            OrderManager.instance.CheckMeal(activeMeal);
            CartAnimator.SetTrigger("Cart Out");
            mealGO.transform.SetParent(cartPos);
            Destroy(mealGO, 0.2f);
            activeMeal = null;
            mealGO = null;
            frogController.SetBool("Has recipe", false);
            belly.Clear();
            bellyDisplay.UpdateUI();
            bellyDisplay.UpdateMealUI(null);

            isThrowingUp = false;
        });
    }

    IEnumerator ThrowUpIngredients()
    {
        Profiler.BeginSample("Kaio Profiller: Throwing Up Ingredients");
        isThrowingUp = true;
        int numIngredients = belly.Count - 1;

        while (tongue.isTongueOccupied)
            yield return null;

        if (activeMeal != null)
        {
            frogController.SetBool("Has recipe", true);
            foreach (var item in belly)
            {
                Destroy(item.gameObject);
            }

            //Spawnar e lancar meal

            MealGO _mealGO = Instantiate(activeMeal.mealGO);
            mealGO = _mealGO.gameObject;
            mealGO.SetActive(false);
            _mealGO.transform.position = bellyPos.transform.position;
            frogController.SetTrigger("Food Out");
            animationController.realayerWeight = 0;

            //Move Meal to Cart agora está sendo comandada por eventos na animação
            //MoveMealToCart();

            //_mealGO.LaunchItSelf(transform.forward);

        }
        else
        {
            while (numIngredients >= 0)
            {

                int rand = UnityEngine.Random.Range(0, swallowClip.Length);
                audioSource.PlayOneShot(swallowClip[rand]);
                Instantiate(saliva_VFX, Jaw_Pos.transform.position, gameObject.transform.rotation);
                animationController.realayerWeight -= 0.25f;
                frogController.SetTrigger("Food Out");
                LaunchIngredient(belly[numIngredients]);
                numIngredients--;
                yield return new WaitForSeconds(0.17f);
            }

            belly.Clear();
            bellyDisplay.UpdateUI();
            bellyDisplay.UpdateMealUI(null);

            isThrowingUp = false;
        }
        
        Profiler.EndSample();
    }



    void LaunchIngredient(IngredientScriptable ingredient)
    {
        ingredient.gameObject.SetActive(true);
        ingredient.transform.SetParent(null);
        ingredient.LaunchItSelf(transform.forward);
    }

    void LaunchMealGO(MealGO mealGo)
    {
        mealGo.LaunchItSelf(transform.forward);
    }

    public bool IsBellyFull()
    {
        return belly.Count - 1 == maxIngredients;
    }

    void OnIngredientAdd()
    {

        timeFoodInBelly -= reduceTimeInBelly;

        if (belly.Count < 2)
        {
            Debug.Log("Not a meal");
            return;
        }

        activeMeal = GetMeal();

        bellyDisplay.UpdateMealUI(activeMeal);

        if (activeMeal != null)
        {
            Debug.Log(activeMeal.name);
        }
        else
        {
            Debug.Log("Not a meal");
        }

    }

    Meal GetMeal()
    {
        Meal meal = null;

        foreach (var item in meals)
        {
            if (item.Match(belly))
            {
                meal = item;
                break;
            }
        }

        return meal;
    }

    public void ResetBellyFrog()
    {
        belly.Clear();
        bellyDisplay.UpdateUI();
        bellyDisplay.UpdateMealUI(null);
        animationController.realayerWeight = 0;
        timeFoodInBelly = 0;

    }

    void CheckFoodInBelly()
    {
        if (belly.Count > 0 && GameManager.instance.gameStates == GameStates.game)
        {
            Profiler.BeginSample("Kaio Profiller: CheckFoodInBelly");
            if (timeFoodInBelly != 0)
            {
                Sweat_VFX.emissionRate = (15 * timeFoodInBelly) / maxTimeInBelly;
            }

            timeFoodInBelly += Time.deltaTime;

            if (timeFoodInBelly >= maxTimeInBelly && !isThrowingUp && !tongue.isTongueOccupied)
            {
                GameManager.instance.ChangeLife(-1);
                ThrowUpAllIngredients();
                timeFoodInBelly = 0;
            }
            Profiler.EndSample();
        }
        else
        {
            Sweat_VFX.emissionRate = 0;
            timeFoodInBelly = 0;
        }

        Profiler.BeginSample("Kaio Profiller: CheckBellyUI");
        timeFoodInBelly = Mathf.Clamp(timeFoodInBelly, 0, maxTimeInBelly);
        UIManager.instance.UpdateBellyFrog(timeFoodInBelly, maxTimeInBelly);
        Profiler.EndSample();

    }

    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtClip);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            if (other.TryGetComponent(out IngredientScriptable ingredientScriptable))
            {
                ingredient = ingredientScriptable;
                ingredient.istargeted = true;
                ingredient.UpdateTargetVFXGO(true);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pickable"))
        {
            if (other.TryGetComponent(out IngredientScriptable ingredientScriptable))
            {
                ingredient = ingredientScriptable;
                ingredient.istargeted = false;
                ingredient.UpdateTargetVFXGO(false);
            }
        }
    }
}
