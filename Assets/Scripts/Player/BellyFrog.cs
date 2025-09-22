using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

public class BellyFrog : MonoBehaviour
{
    public Action<IngredientSo> OnIngredientAdded;
    public Action OnThrowUp;
    public Action<MealSo> OnCheckMeal;
    public Action<MealSo> OnMealDelivered;
    
    [SerializeField] private ParticleSystem saliva_VFX;
    [SerializeField] private ParticleSystem Sweat_VFX;
    [SerializeField] private ParticleSystem StarVFX_GO;
    public GameObject Jaw_Pos;

    public bool targeting;
    private Ingredient ingredient;

    public Animation_Controller animationController;
    public Animator CartAnimator;
    public Animator frogController;
    public BellyDisplay bellyDisplay;
    public List<Ingredient> belly;
    public List<IngredientSo> bellySo;
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
    
    bool isThrowingUp;

    public Transform cartPos;

    public AudioSource audioSource;
    public AudioClip[] swallowClip;
    public AudioClip succesMeal;
    public AudioClip hurtClip;
    
    private Health _health;
    private IEnumerator bellyRoutine;
    private MealPoolManager _mealPool;
    
    private InGameAction launchMealAction;
    private InGameAction launchIngredientAction;
    private InGameAction getIngredientAction;
    private InGameAction generateMealAction;

    [FormerlySerializedAs("eventChannelTutorialAction")]
    [Header("Events")] 
    [SerializeField] private EventChannelAction eventChannelAction;
    
    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animation_Controller>();
        
        launchMealAction = new InGameAction(GameAction.ThrowUp, 1);
        launchIngredientAction = new InGameAction(GameAction.ThrowUp, 1);
        getIngredientAction = new InGameAction(GameAction.GetIngredient, 1);
        generateMealAction = new InGameAction(GameAction.GenerateMeal, 1, mealSo: activeMealSo);
    }

    public void Initialize(GameManager gameManager, Health health)
    {
        OnMealDelivered += gameManager.OrderManager.OnMealDelivered;
        OnCheckMeal += gameManager.OrderManager.CheckMatchMeal;
        gameManager.OrderManager.OnMealDeliveredSuccessfully += OnMealSuccessfully;
        _mealPool = gameManager.ObjectPoolManager.MealPool;
        _health = health;
    }

    private void AddToBelly(Ingredient ingredient)
    {
        belly.Add(ingredient);
        bellySo.Add(ingredient.IngredientSo);
    }

    private void RemoveFromBelly(Ingredient ingredient)
    {
        belly.Remove(ingredient);
        bellySo.Remove(ingredient.IngredientSo);
    }

    private void ClearBelly()
    {
        belly.Clear();
        bellySo.Clear();
    }
    
    public void AddIngredient(Ingredient ingredient)
    {
        if (belly.Count - 1 >= maxIngredients)
            return;

        getIngredientAction.GameAction = GameAction.GetIngredient;
        getIngredientAction.Amount = 1;
        getIngredientAction.IngredientSo = ingredient.IngredientSo;

        eventChannelAction.RaiseEvent(getIngredientAction);
        
        AddToBelly(ingredient);
        animationController.realayerWeight += 0.25f;
        ingredient.gameObject.SetActive(false);
        ingredient.transform.SetParent(bellyPos.transform);
        ingredient.transform.localPosition = Vector3.zero;

        if (ingredient.IngredientSo == rottenFood)
        {
            //Perder vida, cuspir tudo
            _health.TakeDamage(1);
            PlayHurtSound();
            ThrowUpAllIngredients();
            return;
        }

        bellyDisplay.UpdateUI();
        OnIngredientAdd(ingredient);
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
        PlaySalivaVfx();

        OnCheckMeal?.Invoke(activeMealSo);
        mealGO.SetActive(true);
        mealGO.gameObject.transform.position = JawPos.position + PosOffset;
        
        mealGO.transform.DOMove(cartPos.position, 0.1f).OnComplete(() =>
        {
            OnMealDelivered?.Invoke(activeMealSo);
            
            CartAnimator.SetTrigger("Cart Out");
            mealGO.transform.SetParent(cartPos);
            _mealPool.QueueReleaseWithDelay(mealGO, 0.2f);
            activeMealSo = null;
            mealGO = null;
            frogController.SetBool("Has recipe", false);
            ClearBelly();
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
        
        Sweat_VFX.Stop();

        if (activeMealSo != null)
        {
            frogController.SetBool("Has recipe", true);
            foreach (var item in belly)
            {
                item.ReleaseToPool();
            }

            //Spawnar e lancar meal
            
            mealGO = _mealPool.Pool.Get();
            mealGO.SetActive(false);
            mealGO.transform.position = bellyPos.transform.position;
            frogController.SetTrigger("Food Out");
            animationController.realayerWeight = 0;
            launchMealAction.MealSo = activeMealSo;  
            eventChannelAction.RaiseEvent(launchMealAction);

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
                PlaySalivaVfx();
                animationController.realayerWeight -= 0.25f;
                frogController.SetTrigger("Food Out");
                launchIngredientAction.IngredientSo = belly[numIngredients].IngredientSo;  
                eventChannelAction.RaiseEvent(launchIngredientAction);
                LaunchIngredient(belly[numIngredients]);
                numIngredients--;
                yield return new WaitForSeconds(0.17f);
            }

            ClearBelly();
            bellyDisplay.UpdateUI();
            bellyDisplay.UpdateMealUI(null);

            isThrowingUp = false;
        }
        
        OnThrowUp?.Invoke();
    }



    void LaunchIngredient(Ingredient ingredient)
    {
        ingredient.gameObject.SetActive(true);
        ingredient.transform.SetParent(null);
        ingredient.LaunchItSelf(transform.forward);
    }

    private void OnMealSuccessfully()
    {
        StarVFX_GO.Stop();
        StarVFX_GO.Play();
    }

    public bool IsBellyFull()
    {
        return belly.Count - 1 == maxIngredients;
    }

    void OnIngredientAdd(Ingredient ingredient)
    {

        timeFoodInBelly -= reduceTimeInBelly;

        if (bellyRoutine == null)
        {
            bellyRoutine = BellyCounter();
            
            StartCoroutine(bellyRoutine);
        }

        OnIngredientAdded?.Invoke(ingredient.IngredientSo);
        
        if (belly.Count < 2)
        {
            //Debug.Log("Not a meal");
            return;
        }

        activeMealSo = GetMeal();

        generateMealAction.MealSo = activeMealSo;
        eventChannelAction.RaiseEvent(generateMealAction);

        bellyDisplay.UpdateMealUI(activeMealSo);
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
        ClearBelly();
        bellyDisplay.UpdateUI();
        bellyDisplay.UpdateMealUI(null);
        animationController.realayerWeight = 0;
        timeFoodInBelly = 0;

    }

    IEnumerator BellyCounter()
    {
        if (belly.Count <= 0)
            timeFoodInBelly = 0;
        
        Sweat_VFX.Play();
        
        while (belly.Count > 0)
        {
            timeFoodInBelly += Time.deltaTime;
            
            Sweat_VFX.emissionRate = (15 * timeFoodInBelly) / maxTimeInBelly;
            
            if (timeFoodInBelly >= maxTimeInBelly && !isThrowingUp && !tongue.isTongueOccupied)
            {
                _health.TakeDamage(1);
                ThrowUpAllIngredients();
            }
            
            timeFoodInBelly = Mathf.Clamp(timeFoodInBelly, 0, maxTimeInBelly);
            UIManager.instance.UpdateBellyFrog(timeFoodInBelly, maxTimeInBelly);
            
            yield return null;
        }
        
        Sweat_VFX.Stop();
    }

    private void PlaySalivaVfx()
    {
        saliva_VFX.transform.position = Jaw_Pos.transform.position;
        saliva_VFX.transform.rotation = gameObject.transform.rotation;
        saliva_VFX.Stop();
        saliva_VFX.Play();
    }
    
    
    public void PlayHurtSound()
    {
        audioSource.PlayOneShot(hurtClip);
    }
}
