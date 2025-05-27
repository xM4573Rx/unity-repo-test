using UnityEngine;

public class FollowPositionOnly : MonoBehaviour
{
    public Transform target; // El objeto a seguir
    public Vector3 offset = Vector3.zero; // posici�n relativa

    void LateUpdate()
    {
        if (target != null)
        {
            // Solo seguir la posici�n
            transform.position = target.position + offset;

            // mantener rotaci�n fija
            transform.rotation = Quaternion.identity;
        }
    }
}
