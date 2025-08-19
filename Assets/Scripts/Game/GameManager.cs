using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GoogleMobileAds.Api;
using SaveData;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [field: SerializeField] public OrderManager OrderManager { get; private set; }
    [field: SerializeField] public RewardManager RewardManager { get; private set; }
    [field: SerializeField] public ScoreManager ScoreManager { get; private set; }
    [field: SerializeField] public ObjectPoolManager ObjectPoolManager { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }

    public Joystick Joystick => joystick;

    public bool isMobile; //TODO: remover isso quando criar um meio de alternar build mobile e web
    public BellyFrog bellyFrog;
    public Animator an;
    public GameStates gameStates;

    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject actionButton; //TODO: Criar manager de UI
    [SerializeField] private GameObject deliverButton;
    [SerializeField] private Character character;
    [SerializeField] private IngredientSpawner ingredientSpawner;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;
    [SerializeField] private GameObject cameraUI;
    [SerializeField] private DisplayUserInfoUI displayUserInfoUI;
    [SerializeField] private PowerUpManager powerUpManager;
    [SerializeField] private PlayerDataHandler playerDataHandler;
    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private FakePlayerHolderSo fakePlayerHolderSo;
    
    public bool IsGodMode;
    
    private FirebaseDataManager _firebaseDataManager;


    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        _firebaseDataManager = FindAnyObjectByType<FirebaseDataManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();

#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
 Debug.unityLogger.logEnabled = false;
#endif

        ScoreManager.ResetScore();
        //Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 0;

        character.InitializeComponents(this);
        ObjectPoolManager.Initialize();
        ingredientSpawner.Initialize(OrderManager, ObjectPoolManager);
        ScoreManager.Initialize(playerDataHandler, UIManager);
        RewardManager.OnReceivedReward += OnReceivedReward;
        OrderManager.Initialize(ScoreManager, ObjectPoolManager);
        
        if (_firebaseDataManager) //This is for playing game directly from Game scene
            UIManager.LeaderboardUI.InitializeLeaderboard(_firebaseDataManager?.LeaderboardManager);

        foreach (var order in ObjectPoolManager.OrderPool.Orders)
        {
            OrderHighlighter orderHighlighter = order.gameObject.AddComponent<OrderHighlighter>();
            orderHighlighter.Initialize(order, character.BellyFrog);
            order.SetOrderHighlighter(orderHighlighter);
            
            character.BellyFrog.OnIngredientAdded += orderHighlighter.HighLightIngredients;
            character.BellyFrog.OnThrowUp += orderHighlighter.ResetIngredientsColor;
        }
        
        joystick.gameObject.SetActive(false);
        deliverButton.SetActive(false);
        actionButton.SetActive(false);
    }

    private void InitializeComponents()
    {
        if (_firebaseDataManager != null)
            playerDataHandler.Initialize(_firebaseDataManager.PlayerData);
        else
            playerDataHandler.InitializeNotLoggedIn();

        displayUserInfoUI.Initialize(playerDataHandler);
        powerUpManager.Initialize(playerDataHandler);
    }

    public void ChangeQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
        QualitySettings.renderPipeline = qualityLevels[value];
    }

    public void StartGame()
    {
        gameStates = GameStates.game;
        cameraUI.SetActive(false);

        ScoreManager.ResetScore();
        character.Health.ResetLife();
        OrderManager.ResetOrders();

        Ingredient[] ingredients = FindObjectsOfType<Ingredient>();

        foreach (var item in ingredients)
        {
            Destroy(item.gameObject);
        }

        bellyFrog.ResetBellyFrog();
        UIManager.ShowMenu(false);

        an.SetTrigger("Game");

        ingredientSpawner.StartIngredientSpawn();

        AudioManager.instance.PlayGameMusic();

        if (isMobile) //Mover para UIManager?
        {
            joystick.gameObject.SetActive(true);
            deliverButton.SetActive(true);
            actionButton.SetActive(true);
        }
    }
    
    public void LoseGame()
    {
        cameraUI.SetActive(true);
        ingredientSpawner.StopIngredientSpawn();
        var rotationVector = new Vector3(0, 180, 0);
        bellyFrog.gameObject.transform.DORotate(rotationVector, 0.7f, RotateMode.Fast);
        gameStates = GameStates.finish;
        an.SetTrigger("Menu");
        UIManager.ShowHidePostGame(shouldShow: true);
        
        _firebaseDataManager.SavePlayerData();

        OrderManager.ResetOrders();

        joystick.gameObject.SetActive(false);
        deliverButton.SetActive(false);
        actionButton.SetActive(false);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
    }

    public void OnDie()
    {
        if (gameStates is not GameStates.game) return;
        
        ScoreManager.CalculateFinalScore();
        ScoreManager.UpdateScore();
        UIManager.ShowSecondChance(true);
        gameFlowManager.PauseGame(true);
    }
    
    private void OnReceivedReward()
    {
        character.Health.Heal(2);
        gameFlowManager.PauseGame(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

public enum GameStates
{
    menu,
    pause,
    game,
    finish
}