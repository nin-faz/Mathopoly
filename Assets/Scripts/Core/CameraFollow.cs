using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;   // ton pion
    public Vector3 offset;     // décalage (ex: au-dessus et un peu incliné)
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(player); // 👀 garde la caméra tournée vers le joueur
    }
}
