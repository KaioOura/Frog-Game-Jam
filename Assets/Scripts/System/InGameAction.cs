using System;

[Serializable]
public class InGameAction
{
    public string Key => ""; //$"{GameAction}{IngredientSo}{MealSo}"; //Nao mudar a ordem
    public GameAction GameAction;
    public IngredientSo IngredientSo; //Esconder variaveis que nao vao ser usadas na action
    public MealSo MealSo;
    public int Amount;

    public InGameAction(GameAction gameAction, int amount, IngredientSo ingredientSo = null, MealSo mealSo = null)
    {
        this.GameAction = gameAction;
        this.Amount = amount;
        this.IngredientSo = ingredientSo;
        this.MealSo = mealSo;
    }

    public bool CheckKey(InGameAction inGameAction)
    {
        bool ingredientSatisfied = true;
        bool mealSatisfied = true;

        bool gameActionSatisfied = inGameAction.GameAction == GameAction;

        if (!gameActionSatisfied)
            return false;

        if (IngredientSo != null)
            ingredientSatisfied = inGameAction.IngredientSo == IngredientSo;

        if (MealSo != null)
            mealSatisfied = inGameAction.MealSo == MealSo;


        return ingredientSatisfied && mealSatisfied;
    }
}

public enum GameAction
{
    ScreenTouch,
    MoveJoystick,
    SwipeUp,
    SwipeDown,
    SwipeLeft,
    SwipeRight,
    LaunchTongue,
    ThrowUp,
    DeliveryMeal,
    GetIngredient,
    SpawnOrder,
    GenerateMeal,
}