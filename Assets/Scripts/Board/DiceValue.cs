using UnityEngine;

public class DiceValue : MonoBehaviour
{
    public int currentValue = 0;

    public void SetCurrentValue(int value)
    {
        currentValue = value;
    }

    public int GetDiceValue()
    {
        return currentValue;
    }
}