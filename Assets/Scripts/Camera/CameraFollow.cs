using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform diceBoard;
    public Vector3 offset; // décalage (ex: au-dessus et un peu incliné)
    public float smoothSpeed = 5f;

    private Transform currentTarget;
    private Vector3? temporaryTarget = null;

    void Start()
    {
        currentTarget = diceBoard;
        Debug.Log("Camera first on board");
    }

    void LateUpdate()
    {
        if (currentTarget == null) return;

        // Si on a un target temporaire, on le prend, sinon target normal
        Vector3 targetPos = temporaryTarget.HasValue ? temporaryTarget.Value : currentTarget.position;

        transform.position = Vector3.Lerp(transform.position, targetPos + offset, smoothSpeed * Time.deltaTime);
        transform.LookAt(currentTarget);
    }

    // Focus normal
    public void FocusOnPlayer()
    {
        if (player != null)
            currentTarget = player;

        ClearTemporaryTarget();
        Debug.Log("Camera on player");
    }

    public void FocusOnDiceBoard()
    {
        if (diceBoard != null)
            currentTarget = diceBoard;

        ClearTemporaryTarget();
        Debug.Log("Camera on dice board");
    }

    // ✅ Focus temporaire sur une position
    public void SetTemporaryTarget(Vector3 pos)
    {
        temporaryTarget = pos;
    }

    // ✅ Revenir à target normal
    public void ClearTemporaryTarget()
    {
        temporaryTarget = null;
    }
}
