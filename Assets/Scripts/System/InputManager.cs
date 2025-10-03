using System;
using Lean.Touch;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event Action OnTapInput;

    public event Action OnSwipeDownInput;
    
    [SerializeField] private LeanTouch leanTouch;
    [SerializeField] private EventChannelAction eventChannel;
    
    private InGameAction _swipeDownAction;

    private void Start()
    {
        _swipeDownAction = new InGameAction(GameAction.SwipeDown, 1);
    }


    public void Subscribe()
    {
        LeanTouch.OnFingerTap += HandleTap;
        LeanTouch.OnFingerSwipe += HandleSwipeInput;
    }

    public void UnSubscribe()
    {
        LeanTouch.OnFingerTap -= HandleTap;
        LeanTouch.OnFingerSwipe -= HandleSwipeInput;
    }
    
    private void HandleTap(LeanFinger finger)
    {
        if (finger.IsOverGui)
            return;
        
        if (finger.ScreenPosition.x < Screen.width * 0.5f)
            return;
        
        TapInput();
    }
    private void HandleSwipeInput(LeanFinger finger)
    {
        if (finger.ScreenPosition.x < Screen.width * 0.5f)
            return;
        
        Vector2 swipe = finger.SwipeScreenDelta.normalized;
        
        float dot = Vector2.Dot(swipe, Vector2.down);
        if (dot > 0.7f) // 0.9 para ser bem preciso
        {
            OnSlideDownInput();
        }
    }

    public void TapInput()
    {
        OnTapInput?.Invoke();
    }

    public void OnSlideDownInput()
    {
        OnSwipeDownInput?.Invoke();
        
        eventChannel.RaiseEvent(_swipeDownAction);
    }
    
}
