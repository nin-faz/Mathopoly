using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform diceBoard;
    public Vector3 offset; // décalage (ex: au-dessus et un peu incliné)
    public float smoothSpeed = 5f;

    private Transform currentTarget;
    private Vector3? temporaryTargetPos  = null;
    private Quaternion? temporaryTargetRot = null;

    // 🎥 Modes de vue joueur
    private enum CameraMode { TopDown, Isometric }
    private CameraMode currentMode = CameraMode.TopDown;

    void Start()
    {
        currentTarget = diceBoard;
        Debug.Log("Camera first on board");
    }

    void LateUpdate()
    {
         Vector3 targetPos;
        Quaternion targetRot;

        if (temporaryTargetPos.HasValue && temporaryTargetRot.HasValue)
        {
            targetPos = temporaryTargetPos.Value;
            targetRot = temporaryTargetRot.Value;
        }
        else if (currentTarget != null)
        {
            if (currentTarget == player)
            {
                // --- Vue joueur selon le mode ---
                if (currentMode == CameraMode.TopDown)
                {
                    targetPos = currentTarget.position + new Vector3(0, 40, 0);
                    targetRot = Quaternion.Euler(90f, 0f, 0f);
                }
                else // Isometric
                {
                    targetPos = currentTarget.position + new Vector3(-10, 15, -10);
                    targetRot = Quaternion.Euler(45f, 45f, 0f);
                }
            }
            else
            {
                 // --- Vue plateau ---
                targetPos = currentTarget.position + offset;
                targetRot = Quaternion.LookRotation(currentTarget.position - transform.position);
            }
        }
        else return;

        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothSpeed * Time.deltaTime);

        // 🎮 Switch caméra (touche C)
        if (Input.GetKeyDown(KeyCode.C) && currentTarget == player)
        {
            ToggleCameraMode();
        }
    }

    // ✅ Switch entre TopDown et Isometric
    private void ToggleCameraMode()
    {
        if (currentMode == CameraMode.TopDown)
            currentMode = CameraMode.Isometric;
        else
            currentMode = CameraMode.TopDown;
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
    public void SetTemporaryTarget(Vector3 pos, Quaternion rot)
    {
        temporaryTargetPos = pos;
        temporaryTargetRot = rot;
    }

    // ✅ Revenir à target normal
    public void ClearTemporaryTarget()
    {
        temporaryTargetPos = null;
        temporaryTargetRot = null;
    }
}
