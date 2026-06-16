using UnityEngine;

/// <summary>
/// Responsável apenas pela apresentação das orders: lista de UI, animações e
/// áudio de feedback. Mantém o OrderManager focado na lógica de spawn/regras.
/// </summary>
public class OrderPresenter : MonoBehaviour
{
    [SerializeField] private Animator layoutAnim;
    [SerializeField] private Animator boomboxAnim;
    [SerializeField] private AudioClip successOrder;
    [SerializeField] private VerticalUIList verticalUIList;

    public void AddOrder(RectTransform rect)
    {
        verticalUIList.AddUI(rect);
    }

    public void RemoveOrder(RectTransform rect)
    {
        verticalUIList.RemoveUI(rect);
    }

    public void PlayMatchSound()
    {
        AudioManager.instance.PlayAudioOneShot(successOrder);
    }

    public void PlaySuccessFeedback()
    {
        layoutAnim.SetTrigger("Success");
        boomboxAnim.SetTrigger("Pulo");
    }
}
