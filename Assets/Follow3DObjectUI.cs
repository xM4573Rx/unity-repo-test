using UnityEngine;

public class UIPanelFollower : MonoBehaviour
{
    public Transform target3D;             // Cubo u objeto 3D
    public RectTransform uiElement;       // Panel en el Canvas
    public Camera mainCamera;             // Cámara principal
    public Vector3 screenOffset;          // Opcional: para que no se encime

    void Update()
    {
        if (target3D == null || mainCamera == null || uiElement == null)
            return;

        // Posición en pantalla del objeto 3D
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target3D.position);

        // Si el objeto está detrás de la cámara, ocultamos el panel
        if (screenPos.z < 0)
        {
            uiElement.gameObject.SetActive(false);
            return;
        }

        uiElement.gameObject.SetActive(true);

        // Aplicamos posición con offset opcional
        uiElement.position = screenPos + screenOffset;
    }
}
