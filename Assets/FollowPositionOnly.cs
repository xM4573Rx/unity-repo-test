using UnityEngine;

public class FollowPositionOnly : MonoBehaviour
{
    public Transform target; // El objeto a seguir
    public Vector3 offset = Vector3.zero; // posición relativa

    void LateUpdate()
    {
        if (target != null)
        {
            // Solo seguir la posición
            transform.position = target.position + offset;

            // mantener rotación fija
            transform.rotation = Quaternion.identity;
        }
    }
}
