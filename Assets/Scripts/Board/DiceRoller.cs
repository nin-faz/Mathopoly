using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    private Rigidbody diceRigidbody;

    public float launchForce = 10f;
    public float launchTorque = 50f;

    public PlayerMovement player;
    public CameraFollow mainCamera;
    public Transform diceSpawn;
    public Button rollDiceButton;

    // ✅ Méthode pour passer les références depuis GameManager
    public void Initialize(PlayerMovement player, CameraFollow mainCamera, Transform diceSpawn, Button rollDiceButton)
    {
        this.player = player;
        this.mainCamera = mainCamera;
        this.diceSpawn = diceSpawn;
        this.rollDiceButton = rollDiceButton;
    }

    void Awake()
    {
        diceRigidbody = GetComponent<Rigidbody>();
    }

    public void RollDice()
    {
        // Caméra sur le plateau de dé
        if (mainCamera != null)
            mainCamera.FocusOnDiceBoard();

        // Lancer le dé
        StartCoroutine(ThrowDice());
    }

    private void SpawnDice()
    {
        diceRigidbody.position = diceSpawn.position;
        diceRigidbody.rotation = Random.rotation;
        diceRigidbody.linearVelocity = Vector3.zero;
        diceRigidbody.angularVelocity = Vector3.zero;

        diceRigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        diceRigidbody.AddTorque(Random.insideUnitSphere * launchTorque, ForceMode.Impulse);
    }

    private IEnumerator ThrowDice()
    {
        // Désactive le bouton pour éviter les lancers multiples
        if (rollDiceButton != null)
            rollDiceButton.gameObject.SetActive(false);
        
        bool diceStable = false;

        while (!diceStable)
        {
            SpawnDice();

            // ⚡ On attend un petit temps minimum pour que le dé soit visible
            yield return new WaitForSeconds(1.5f);

            // ⚡ On attend qu’il s'arrête ou qu'il tombe
            yield return new WaitUntil(() => {
                return diceRigidbody.position.y < -1f || (diceRigidbody.linearVelocity.magnitude < 0.01f && 
                        diceRigidbody.angularVelocity.magnitude < 0.01f);
            });

            // Si le dé est tombé, on le relance
            if (diceRigidbody.position.y < -1f) 
            {
                continue;
            }

            // Si le dé est coincé de façon incliné alors on le remet à plat
            float angleWithUp = Vector3.Angle(diceRigidbody.transform.up, Vector3.up);
            if (angleWithUp > 15f)
            {
                PlaceDiceFlat();
            }

            diceStable = true;
        }

        int diceValue = GetComponent<DiceValue>().GetDiceValue();

        // On fait un focus temporaire au-dessus du dé
        if (mainCamera != null)
        {
            Vector3 diceCamPos = new Vector3(191.5f, 40f, -32.5f);
            Quaternion diceCamRot = Quaternion.Euler(90f, 90f, 90f);
            mainCamera.SetTemporaryTarget(diceCamPos, diceCamRot);
        }
        yield return new WaitForSeconds(2f);

        if (mainCamera != null)
            mainCamera.FocusOnPlayer();
        
        // Avancer le joueur
        if (player != null)
        {
            yield return new WaitForSeconds(1f);
            player.OnMoveFinished += OnPlayerMoveFinished;
            player.MovePlayer(diceValue);
        }
    }

    private void PlaceDiceFlat()
    {
        Vector3[] faceNormals = new Vector3[]
        {
            Vector3.up, Vector3.down,
            Vector3.left, Vector3.right,
            Vector3.forward, Vector3.back
        };

        Vector3 closestFace = faceNormals[0];
        float maxDot = Vector3.Dot(diceRigidbody.transform.up, faceNormals[0]);

        for (int i = 1; i < faceNormals.Length; i++)
        {
            float dot = Vector3.Dot(diceRigidbody.transform.up, faceNormals[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                closestFace = faceNormals[i];
            }
        }

        Quaternion targetRotation = Quaternion.FromToRotation(diceRigidbody.transform.up, closestFace) * diceRigidbody.transform.rotation;
        diceRigidbody.transform.rotation = targetRotation;
    }

    private void OnPlayerMoveFinished()
    {
        // caméra retourne sur le plateau de dé et i
        if (mainCamera != null)
        {
            Invoke(nameof(FocusDiceBoardWithDelay), 0.5f);
            if (rollDiceButton != null)
                Invoke(nameof(ShowRollDiceButton), 1f);
        }

        Destroy(gameObject, 1f);

        // on se désabonne pour ne pas empiler les callbacks
        if (player != null)
            player.OnMoveFinished -= OnPlayerMoveFinished;        
    }

    private void FocusDiceBoardWithDelay()
    {
        mainCamera.FocusOnDiceBoard();
    }

    private void ShowRollDiceButton()
    {
        rollDiceButton.gameObject.SetActive(true);
    }
}
 