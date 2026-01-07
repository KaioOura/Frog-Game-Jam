using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

public class BellyFrog : MonoBehaviour
{
    public Action OnUpdateIngredients;
    public Action<MealSo> OnCheckMeal;
    public Action<MealSo> OnMealDelivered;
    public Action<List<Ingredient>, MealSo> OnUpdateBellyUI;
    
    public BellyInventory BellyInventory => bellyInventory;
    
    private static readonly int Damage = Animator.StringToHash("Damage");
    private static readonly int CartOut = Animator.StringToHash("Cart Out");
    private static readonly int HasRecipe = Animator.StringToHash("Has recipe");
    private static readonly int FoodOut = Animator.StringToHash("Food Out");
    
    [SerializeField] private ParticleSystem saliva_VFX;
    [SerializeField] private ParticleSystem Sweat_VFX;
    [SerializeField] private ParticleSystem StarVFX_GO;
    [SerializeField] private ParticleSystem explosionVFX;
    public GameObject Jaw_Pos;

    public bool targeting;
    private Ingredient ingredient;

    public Animation_Controller animationController;
    public Animator CartAnimator;
    public Animator frogController;
    [SerializeField] private BellyInventory bellyInventory;
    public Transform bellyPos, JawPos;
    public Tongue tongue;
    public List<MealSo> meals;
    [SerializeField] private MealSo invalidMeal;

    public MealSo activeMealSo;
    GameObject mealGO;
    [SerializeField] private IngredientSo rottenFood;

    // public float maxTimeInBelly;
    // public float timeFoodInBelly;
    // public float reduceTimeInBelly;
    
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
        bellyInventory.AddIngredient(ingredient);
    }

    private void LaunchFromBelly(int slotIndex)
    {
        launchIngredientAction.IngredientSo = bellyInventory.Belly[slotIndex].IngredientSo;  
        eventChannelAction.RaiseEvent(launchIngredientAction);
        
        LaunchIngredient(bellyInventory.Belly[slotIndex]);
        //fazer o sapo cuspir o ingrediente correto e depois chamar esse método abaixo
        bellyInventory.RemoveIngredient(slotIndex);
    }
    private void RemoveFromBelly(int slotIndex)
    {
        bellyInventory.Belly[slotIndex].ReleaseToPool();
        bellyInventory.RemoveIngredient(slotIndex);
    }

    private void ClearBelly()
    {
        bellyInventory.ClearBelly();
    }
    
    public void AddIngredient(Ingredient ingredient)
    {
        if (bellyInventory.GetBellyCount() >= bellyInventory.MaxIngredients)
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
            StartCoroutine(HandleDamageFeedbackRoutine());
            return;
        }

        //OnUpdateBellyUI?.Invoke(bellyInventory.GetIngredients(), activeMealSo);
        OnIngredientAdd();
    }

    private IEnumerator HandleDamageFeedbackRoutine()
    {
        frogController.SetTrigger(Damage);

        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => frogController.GetCurrentAnimatorStateInfo(0).IsName("Damage"));
        PlayHurtFeedback();
        yield return new WaitUntil(() => !frogController.GetCurrentAnimatorStateInfo(0).IsName("Damage"));
        
        DeleteAllIngredients();
    }
    
    public void ThrowIngredient(int slotIndex)
    {
        if (!CanThrowUp())
            return;

        StopBellyRoutine();

        bellyRoutine = ThrowUpSingleIngredient(slotIndex);
        StartCoroutine(bellyRoutine);
    }
    
    public void ThrowUpAllIngredients()
    {
        if (!CanThrowUp())
            return;

        StopBellyRoutine();

        bellyRoutine = ThrowUpIngredients();
        StartCoroutine(bellyRoutine);
    }

    private void DeleteAllIngredients()
    {
        for (int i = 0; i < bellyInventory.Belly.Count; i++)
        {
            if (bellyInventory.Belly[i] == null) continue;
            
            RemoveFromBelly(i);
        }
        
        OnUpdateBellyUI?.Invoke(bellyInventory.Belly, activeMealSo);
        OnUpdateIngredients?.Invoke();
    }

    private bool CanThrowUp() =>
        !isThrowingUp && !tongue.isTongueOccupied;

    private void StopBellyRoutine()
    {
        if (bellyRoutine != null)
        {
            StopCoroutine(bellyRoutine);
            bellyRoutine = null;
        }
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
            
            CartAnimator.SetTrigger(CartOut);
            mealGO.transform.SetParent(cartPos);
            _mealPool.QueueReleaseWithDelay(mealGO, 0.2f);
            activeMealSo = null;
            mealGO = null;
            frogController.SetBool(HasRecipe, false);
            ClearBelly();
            OnUpdateBellyUI?.Invoke(bellyInventory.Belly, activeMealSo);
            OnUpdateIngredients?.Invoke();

            isThrowingUp = false;
        });
    }

    private IEnumerator ThrowUpSingleIngredient(int slotIndex)
    {
        isThrowingUp = true;

        while (tongue.isTongueOccupied)
            yield return null;

        LaunchFromBelly(slotIndex);
        IngredientThrowFeedback();

        yield return new WaitForSeconds(0.17f);

        activeMealSo = GetMeal();
        
        OnUpdateBellyUI?.Invoke(bellyInventory.Belly, activeMealSo);
        OnUpdateIngredients?.Invoke();
        
        isThrowingUp = false;
    }
    
    private IEnumerator ThrowUpIngredients()
    {
        isThrowingUp = true;

        while (tongue.isTongueOccupied)
            yield return null;

        yield return HandleThrow();
        
        OnUpdateIngredients?.Invoke();
        isThrowingUp = false;
    }

    private IEnumerator HandleThrow()
    {
        if (activeMealSo != null && activeMealSo != invalidMeal)
        {
            yield return ThrowMealRecipe();
            
            yield return new WaitForSeconds(0.17f);
        }
        else
        {
            yield return ThrowIngredientsOneByOne();
            
            yield return new WaitForSeconds(0.17f);
            
            activeMealSo = null;
            OnUpdateBellyUI?.Invoke(bellyInventory.Belly, activeMealSo);
        }
    }

    private IEnumerator ThrowMealRecipe()
    {
        frogController.SetBool(HasRecipe, true);

        foreach (var ingredient in bellyInventory.GetIngredients())
            ingredient.ReleaseToPool();

        mealGO = _mealPool.Pool.Get();
        mealGO.SetActive(false);
        mealGO.transform.position = bellyPos.transform.position;

        frogController.SetTrigger(FoodOut);
        animationController.realayerWeight = 0;

        launchMealAction.MealSo = activeMealSo;
        eventChannelAction.RaiseEvent(launchMealAction);
        
        yield break;
    }

    private IEnumerator ThrowIngredientsOneByOne()
    {
        var ingredients = bellyInventory.GetIngredients();

        while (ingredients.Count > 0)
        {
            for (int i = 0; i < bellyInventory.Belly.Count; i++)
            {
                if (bellyInventory.Belly[i] != null)
                {
                    LaunchFromBelly(i);
                    break;
                }
            }

            IngredientThrowFeedback();

            yield return new WaitForSeconds(0.17f);

            ingredients = bellyInventory.GetIngredients();
        }
    }
    
    private void IngredientThrowFeedback()
    {
        int rand = UnityEngine.Random.Range(0, swallowClip.Length);
        audioSource.PlayOneShot(swallowClip[rand]);

        PlaySalivaVfx();
        animationController.realayerWeight -= 0.25f;
        frogController.SetTrigger(FoodOut);
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
        return bellyInventory.IsFull();
    }

    void OnIngredientAdd()
    {
        
        OnUpdateIngredients?.Invoke();
        
        activeMealSo = GetMeal();

        OnUpdateBellyUI?.Invoke(bellyInventory.GetIngredients(), activeMealSo);
        
        if (activeMealSo == null)
            return;
        
        generateMealAction.MealSo = activeMealSo;
        eventChannelAction.RaiseEvent(generateMealAction);
    }

    MealSo GetMeal()
    {
        if (bellyInventory.GetIngredients().Count < 2) return null;
        MealSo mealSo = meals.FirstOrDefault(item => item.Match(bellyInventory.GetIngredients()));
        
        return mealSo != null ? mealSo : invalidMeal;
    }

    public void ResetBellyFrog()
    {
        ClearBelly();
        OnUpdateBellyUI?.Invoke(bellyInventory.GetIngredients(), activeMealSo);
        animationController.realayerWeight = 0;
        //timeFoodInBelly = 0;

    }

    // IEnumerator BellyCounter()
    // {
    //     if (bellyInventory.GetIngredients().Count <= 0)
    //         timeFoodInBelly = 0;
    //     
    //     Sweat_VFX.Play();
    //     
    //     while (bellyInventory.GetIngredients().Count > 0)
    //     {
    //         timeFoodInBelly += Time.deltaTime;
    //         
    //         Sweat_VFX.emissionRate = (15 * timeFoodInBelly) / maxTimeInBelly;
    //         
    //         if (timeFoodInBelly >= maxTimeInBelly && !isThrowingUp && !tongue.isTongueOccupied)
    //         {
    //             _health.TakeDamage(1);
    //             ThrowUpAllIngredients();
    //         }
    //         
    //         timeFoodInBelly = Mathf.Clamp(timeFoodInBelly, 0, maxTimeInBelly);
    //         UIManager.instance.UpdateBellyFrog(timeFoodInBelly, maxTimeInBelly);
    //         
    //         yield return null;
    //     }
    //     
    //     Sweat_VFX.Stop();
    // }

    private void PlaySalivaVfx()
    {
        saliva_VFX.transform.position = Jaw_Pos.transform.position;
        saliva_VFX.transform.rotation = gameObject.transform.rotation;
        saliva_VFX.Stop();
        saliva_VFX.Play();
    }
    
    
    public void PlayHurtFeedback()
    {
        audioSource.PlayOneShot(hurtClip);
        explosionVFX.Play();
    }
}
