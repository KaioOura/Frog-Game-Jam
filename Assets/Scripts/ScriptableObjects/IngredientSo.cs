using UnityEngine;

[CreateAssetMenu(fileName = "IngredientSo", menuName = "Scriptable Objects/IngredientSo")]
public class IngredientSo : ScriptableObject
{
    public Sprite myImage;
    public string myName;
    public int baseWeight = 5;
    public Ingredient ingredientPrefab;
}

public enum Difficulty
{
    easy,
    normal,
    hard
}