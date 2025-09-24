using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public CharState CharState => _charState;
    
    private CharState _charState;
    
    
    public void ChangeState(CharState newState)
    {
        _charState = newState;
    }
}
