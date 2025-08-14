using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

public class BellyFrog : MonoBehaviour
{
    public Action<MealSo> OnCheckMeal;
    public Action<MealSo> OnMealDelivered;
    
    public GameObject saliva_VFX, Jaw_Pos;
    public ParticleSystem Sweat_VFX;

    public bool targeting;
    private Ingredient ingredient;

    public Animation_Controller animationController;
    public Animator CartAnimator;
    public Animator frogController;
    public BellyDisplay bellyDisplay;
    public List<Ingredient> belly;
    public int maxIngredients;
    public Transform bellyPos, JawPos;
    public Tongue tongue;
    public List<MealSo> meals;

    [FormerlySerializedAs("activeMeal")] public MealSo activeMealSo;
    GameObject mealGO;
    [SerializeField] private IngredientSo rottenFood;

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

    private Health _health;
    private IEnumerator bellyRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animation_Controller>();
    }

    public void Initialize(GameManager gameManager, Health health)
    {
        OnMealDelivered += gameManager.OrderManager.OnMealDelivered;
        OnCheckMeal += gameManager.OrderManager.CheckMatchMeal;
        _health = health;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        if (belly.Count - 1 >= maxIngredients)
            return;

        belly.Add(ingredient);
        animationController.realayerWeight += 0.25f;
        ingredient.gameObject.SetActive(false);
        ingredient.transform.SetParent(bellyPos.transform);
        ingredient.transform.localPosition = Vector3.zero;

        if (ingredient.IngredientSo == rottenFood)
        {
            //Perder vida, cuspir tudo
            _health.TakeDamage(-1);
            PlayHurtSound();
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

        timeFoodInBelly = 0;

        timeFoodInBelly = Mathf.Clamp(timeFoodInBelly, 0, maxTimeInBelly);
        UIManager.instance.UpdateBellyFrog(timeFoodInBelly, maxTimeInBelly);
        
        if (bellyRoutine != null)
        {
            StopCoroutine(bellyRoutine);
            bellyRoutine = null;
        }
        
        StartCoroutine(ThrowUpIngredients());
    }

    public void MoveMealToCart(Vector3 PosOffset)
    {
        int rand = UnityEngine.Random.Range(0, swallowClip.Length);
        audioSource.PlayOneShot(swallowClip[rand]);
        Instantiate(saliva_VFX, Jaw_Pos.transform.position, gameObject.transform.rotation);
        OnCheckMeal?.Invoke(activeMealSo);
        mealGO.SetActive(true);
        mealGO.gameObject.transform.position = JawPos.position + PosOffset;
        
        mealGO.transform.DOMove(cartPos.position, 0.1f).OnComplete(() =>
        {
            OnMealDelivered?.Invoke(activeMealSo);
            
            CartAnimator.SetTrigger("Cart Out");
            mealGO.transform.SetParent(cartPos);
            Destroy(mealGO, 0.2f);
            activeMealSo = null;
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
        isThrowingUp = true;
        int numIngredients = belly.Count - 1;

        while (tongue.isTongueOccupied)
            yield return null;

        if (activeMealSo != null)
        {
            frogController.SetBool("Has recipe", true);
            foreach (var item in belly)
            {
                Destroy(item.gameObject);
            }

            //Spawnar e lancar meal

            MealGO _mealGO = Instantiate(activeMealSo.mealGO);
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
    }



    void LaunchIngredient(Ingredient ingredient)
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

        if (bellyRoutine == null)
        {
            bellyRoutine = BellyCounter();
            
            StartCoroutine(bellyRoutine);
        }

        if (belly.Count < 2)
        {
            Debug.Log("Not a meal");
            return;
        }

        activeMealSo = GetMeal();

        bellyDisplay.UpdateMealUI(activeMealSo);

        if (activeMealSo != null)
        {
            Debug.Log(activeMealSo.name);
        }
        else
        {
            Debug.Log("Not a meal");
        }

    }

    MealSo GetMeal()
    {
        MealSo mealSo = null;

        foreach (var item in meals)
        {
            if (item.Match(belly))
            {
                mealSo = item;
                break;
            }
        }

        return mealSo;
    }

    public void ResetBellyFrog()
    {
        belly.Clear();
        bellyDisplay.UpdateUI();
        bellyDisplay.UpdateMealUI(null);
        animationController.realayerWeight = 0;
        timeFoodInBelly = 0;

    }

    IEnumerator BellyCounter()
    {
        if (belly.Count <= 0)
            timeFoodInBelly = 0;
        
        while (belly.Count > 0)
        {
            timeFoodInBelly += Time.deltaTime;
            
            //Sweat_VFX.emissionRate = (15 * timeFoodInBelly) / maxTimeInBelly;
            
            if (timeFoodInBelly >= maxTimeInBelly && !isThrowingUp && !tongue.isTongueOccupied)
            {
                _health.TakeDamage(-1);
                ThrowUpAllIngredients();
            }
            
            timeFoodInBelly = Mathf.Clamp(timeFoodInBelly, 0, maxTimeInBelly);
            UIManager.instance.UpdateBellyFrog(timeFoodInBelly, maxTimeInBelly);
            
            yield return null;
        }
    }

    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtClip);
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Pickable"))
    //     {
    //         if (other.TryGetComponent(out Ingredient ingredientScriptable))
    //         {
    //             ingredient = ingredientScriptable;
    //             ingredient.UpdateTargetVFXGO(true);
    //         }
    //
    //     }
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Pickable"))
    //     {
    //         if (other.TryGetComponent(out Ingredient ingredientScriptable))
    //         {
    //             ingredient = ingredientScriptable;
    //             ingredient.UpdateTargetVFXGO(false);
    //         }
    //     }
    // }
}
