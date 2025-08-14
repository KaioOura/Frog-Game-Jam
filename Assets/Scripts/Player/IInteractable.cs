using UnityEngine;

public interface IInteractable
{
   public void OnInteract();
   public void OnSelected();
   public void OnDeselected();
   public void OnInteractEnded();
}
