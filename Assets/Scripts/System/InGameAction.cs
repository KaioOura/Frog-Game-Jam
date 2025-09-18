
namespace System
{
    [Serializable]
    public class InGameAction
    {
        public string Key => $"{GameAction}_{IngredientSo}_{MealSo}"; //Nao mudar a ordem
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
    }
    
    public enum GameAction
    {
        ScreenTouch,
        MoveJoystick,
        LaunchTongue,
        ThrowUp,
        DeliveryMeal,
        GetIngredient,
    }
}