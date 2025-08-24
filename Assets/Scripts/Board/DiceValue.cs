using UnityEngine;

public class DiceValue : MonoBehaviour
{
    public int currentValue = 0;

    public void SetCurrentValue(int value)
    {
        currentValue = value;
        Debug.Log("Valeur du dé détectée : " + value);
    }

    public int GetDiceValue()
    {
        return currentValue;
    }
}