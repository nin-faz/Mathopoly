using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject dicePrefab;
    public Transform diceSpawn;
    public CameraFollow cameraFollow;
    public PlayerMovement player;
    public Button rollDiceButton;

    void Start()
    {
        cameraFollow.FocusOnDiceBoard();
    }

    // Méthode appelée par le bouton
    public void OnRollDiceButton()
    {
        // Instancie le dé
        GameObject diceObj = Instantiate(dicePrefab);
        DiceRoller diceScript = diceObj.GetComponent<DiceRoller>();

        // ⚡ Passe les références
        diceScript.Initialize(player, cameraFollow, diceSpawn, rollDiceButton);

        // Ensuite seulement on lance le dé
        diceScript.RollDice();
    }
}
