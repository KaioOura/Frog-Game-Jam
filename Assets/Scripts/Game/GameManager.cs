using System;
using UnityEngine;
using DG.Tweening;
using Firebase.Analytics;
using Firebase.Firestore;
using SaveData;
using UnityEngine.Rendering;
using Application = UnityEngine.Application;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [field: SerializeField] public OrderManager OrderManager { get; private set; }
    [field: SerializeField] public RewardManager RewardManager { get; private set; }
    [field: SerializeField] public ScoreManager ScoreManager { get; private set; }
    [field: SerializeField] public ObjectPoolManager ObjectPoolManager { get; private set; }
    [field: SerializeField] public ScoreUI ScoreUI { get; private set; }
    [field: SerializeField] public HealthUI HealthUI { get; private set; }
    [field: SerializeField] public LeaderboardUI LeaderboardUI { get; private set; }
    [field: SerializeField] public MobileInputUI MobileInputUI { get; private set; }
    [field: SerializeField] public BellyDisplayUI BellyDisplayUI { get; private set; }
    [field: SerializeField] public TimeScaler TimeScaler { get; private set; }
    [field: SerializeField] public TutorialManager TutorialManager { get; private set; }
    [field: SerializeField] public ActionManager ActionManager { get; private set; }
    [field: SerializeField] public DifficultyManager DifficultyManager { get; private set; }
    
    
    public bool isMobile; //TODO: remover isso quando criar um meio de alternar build mobile e web
    public Animator an;
    public GameStates gameStates;


    [SerializeField] private GameObject actionButton; //TODO: Criar manager de UI
    [SerializeField] private GameObject deliverButton;
    [SerializeField] private Character character;
    [SerializeField] private IngredientSpawner ingredientSpawner;
    [SerializeField] private ConveyorManager conveyorManager;
    [SerializeField] private GameObject cameraUI;
    [SerializeField] private DisplayUserInfoUI displayUserInfoUI;
    [SerializeField] private PowerUpManager powerUpManager;
    [SerializeField] private PlayerDataHandler playerDataHandler;
    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private FakePlayerHolderSo fakePlayerHolderSo;
    [SerializeField] private InputManager inputManager;

    
    private FirebaseDataManager _firebaseDataManager;
    private int _startGameTime;
    
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
        ingredientSpawner.Initialize(OrderManager, ObjectPoolManager, conveyorManager);
        ScoreManager.Initialize(playerDataHandler, ScoreUI);
        RewardManager.OnReceivedReward += OnReceivedReward;
        OrderManager.Initialize(ScoreManager, ObjectPoolManager, DifficultyManager);
        conveyorManager.Initialize(DifficultyManager);

        inputManager.OnTapInput += character.PlayerTongueAction.LaunchTongue;
        inputManager.OnSwipeDownInput += character.BellyFrog.ThrowUpAllIngredients;
        
        if (_firebaseDataManager) //This is for playing game directly from Game scene
            LeaderboardUI.InitializeLeaderboard(_firebaseDataManager?.LeaderboardManager);

        BellyDisplayUI.Initialize(character.BellyFrog);

        foreach (var order in ObjectPoolManager.OrderPool.Orders)
        {
            OrderHighlighter orderHighlighter = order.gameObject.AddComponent<OrderHighlighter>();
            orderHighlighter.Initialize(order, character.BellyFrog);
            order.SetOrderHighlighter(orderHighlighter);
            
            character.BellyFrog.OnUpdateIngredients += orderHighlighter.UpdateOrderIngredients;
        }
        
        gameFlowManager.Initialize(TimeScaler);
        TutorialManager.Initialize(this, playerDataHandler);
        ActionManager.Initialize(playerDataHandler);
        OrderManager.OnOrderSpawned += ActionManager.OnOrderSpawned;
    }

    private void InitializeComponents()
    {
        if (_firebaseDataManager != null)
            playerDataHandler.Initialize(_firebaseDataManager.PlayerData);
        else
            playerDataHandler.InitializeNotLoggedIn();

        displayUserInfoUI.Initialize(playerDataHandler);
        powerUpManager.Initialize(playerDataHandler);
        MobileInputUI.Initialize(inputManager);
    }
    
    public void StartGame()
    {
        gameStates = GameStates.game;
        cameraUI.SetActive(false);

        ScoreManager.ResetScore();
        character.Health.ResetLife();
        OrderManager.ResetOrders();
        DifficultyManager.ResetDifficulty();

        Ingredient[] ingredients = FindObjectsByType<Ingredient>((FindObjectsSortMode)FindObjectsInactive.Include);

        foreach (var item in ingredients)
        {
            Destroy(item.gameObject);
        }

        character.BellyFrog.ResetBellyFrog();
        ScreenManager.Instance.Open(ScreenId.Game);
        MobileInputUI.ShowUI(true);
        BellyDisplayUI.ResetUI();

        an.SetTrigger("Game");
        

        AudioManager.instance.PlayGameMusic();
        //TutorialManager.TryStartTutorial();

        _startGameTime = (int)Time.time;
        
        GameAnalyticsManager.Track("game_start");
    }
    
    public void LoseGame()
    {
        cameraUI.SetActive(true);
        //ingredientSpawner.StopIngredientSpawn();
        var rotationVector = new Vector3(0, 180, 0);
        character.BellyFrog.gameObject.transform.DORotate(rotationVector, 0.7f, RotateMode.Fast);
        gameStates = GameStates.finish;
        an.SetTrigger("Menu");
        //ScreenManager.Instance.Open(ScreenId.PostGame);

        if (_firebaseDataManager)
            _firebaseDataManager.SavePlayerData();

        OrderManager.ResetOrders();
        DifficultyManager.ResetDifficulty();

        MobileInputUI.ShowUI(false);
        
        int timePlayed = (int)(Time.time - _startGameTime);
        GameAnalyticsManager.Track("game_end", 
            ParametersGetter.GetDieParameters(playerDataHandler, DifficultyManager, ScoreManager, timePlayed, OrderManager));
        
        GoToMenu();
    }

    public void GoToMenu()
    {
        TimeScaler.ShouldStopTime(false);
        ScreenManager.Instance.Open(ScreenId.Menu);
    }

    public void OnDie()
    {
        if (gameStates is not GameStates.game) return;
        
        ScoreManager.CalculateFinalScore();
        ScoreManager.UpdateScore();
        ScreenManager.Instance.Open(ScreenId.SecondChance);
        gameFlowManager.PauseGame(true);

       
        int timePlayed = (int)(Time.time - _startGameTime);
        GameAnalyticsManager.Track("onDie", 
            ParametersGetter.GetDieParameters(playerDataHandler, DifficultyManager, ScoreManager, timePlayed, OrderManager));
    }
    
    private void OnReceivedReward()
    {
        character.Health.Heal(2);
        gameFlowManager.PauseGame(false);
        OrderManager.FreezeActiveOrders();
        GameAnalyticsManager.Track("rewarded_ad_received");
    }

    public void OnTutorialEnded()
    {
        playerDataHandler.Tutorial.SetTutorial(false);
        if (_firebaseDataManager != null)
            _firebaseDataManager.SavePlayerData();
    }

    public void PauseGame(bool shouldPause)
    {
        gameFlowManager.PauseGame(shouldPause);
        
        if (shouldPause)
            ScreenManager.Instance.Open(ScreenId.Pause);
        else
            ScreenManager.Instance.CloseTop();
    }

    public void OpenSettings(bool shouldOpen)
    {
        if (shouldOpen)
            ScreenManager.Instance.Open(ScreenId.Settings);
        else
            ScreenManager.Instance.CloseTop();
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