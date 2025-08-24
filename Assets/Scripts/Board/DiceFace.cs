using UnityEngine;

public class DiceFace : MonoBehaviour
{
    public int value; // La valeur de la face du dessus quand cette face touche le sol

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DiceBoard"))
        {
            // On va chercher le DiceValue sur le parent
            DiceValue diceValue = GetComponentInParent<DiceValue>();
            if (diceValue != null)
            {
                diceValue.SetCurrentValue(value);
            }
        }
    }
}