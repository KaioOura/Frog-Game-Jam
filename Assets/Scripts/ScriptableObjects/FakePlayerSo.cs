using UnityEngine;

[CreateAssetMenu(fileName = "FakePlayerSo", menuName = "Scriptable Objects/FakePlayerSo")]
public class FakePlayerSo : ScriptableObject
{
    public string playerName;
    public int highScore;
}
