using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private Transform playerTr;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private LineRenderer interactLine;

    [Header("Debug")]
    public bool showDebugRay = true;

    private IInteractable currentTarget;
    private Vector3 endPointPos;

    private void Start()
    {
        interactLine.SetPosition(0, new Vector3(transform.position.x, 2.2f, transform.position.z));
        
        endPointPos = new Vector3(transform.position.x, 2.2f, transform.position.z);
    }

    private void Update()
    {
        Ray ray = new Ray(playerTr.transform.position, playerTr.transform.forward);
        RaycastHit hit;

        if (showDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.yellow);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            if (!hit.transform.TryGetComponent(out IInteractable interactable)) return;
            
            interactLine.SetPosition(1, new Vector3(hit.point.x, endPointPos.y, hit.point.z));

            // Só reage se for diferente do último
            if (interactable == currentTarget) return;
            
            currentTarget?.OnDeselected();
            
            currentTarget = interactable;
            
            if (currentTarget != null)
            {
                currentTarget.OnSelected();
            }
        }
        else
        {
            if (currentTarget != null)
            {
                // Parou de olhar
                //Debug.Log("Parou de olhar para " + currentTarget.name);
                currentTarget.OnDeselected();
                currentTarget = null;
            }
            
            interactLine.SetPosition(1,  endPointPos + transform.forward * interactionDistance);
        }
    }
}
