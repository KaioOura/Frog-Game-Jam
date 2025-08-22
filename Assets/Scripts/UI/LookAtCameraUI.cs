using UnityEngine;

public class LookAtCameraUI : MonoBehaviour
{
    public Camera targetCamera;

    [Header("Axis Lock")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // Direção para a câmera
        Vector3 direction = targetCamera.transform.position - transform.position;

        // Remove eixos bloqueados
        if (lockX) direction.x = 0f;
        if (lockY) direction.y = 0f;
        if (lockZ) direction.z = 0f;

        // Se a direção virar zero, não rotaciona
        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(-direction); 
        }
    }
}