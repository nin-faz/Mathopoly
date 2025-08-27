using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using TMPro; // pour TextMeshPro

[ExecuteInEditMode]
public class BoardBuilder : MonoBehaviour
{
    [Header("Références")]
    public Transform waypointsParent;   
    public GameObject squarePrefab;     
    public Material[] squareMaterials;  
    public GameObject numberPrefab;     // Prefab avec un TextMeshPro pour le numéro

    [Header("Paramètres")]
    public int blockSize = 5;           
    public Vector3 numberOffset = new Vector3(0, 0.2f, 0); // légèrement au-dessus de la case

    [ContextMenu("Build Board")]
    public void BuildBoard()
    {
        if (waypointsParent == null || squarePrefab == null || squareMaterials.Length == 0 || numberPrefab == null)
        {
            Debug.LogWarning("Références manquantes !");
            return;
        }

        int colorIndex = 0;

        for (int i = 0; i < waypointsParent.childCount; i++)
        {
            Transform waypoint = waypointsParent.GetChild(i);

            // Crée la case
            GameObject newSquare = Instantiate(squarePrefab, waypoint.position, Quaternion.identity);
            newSquare.transform.SetParent(waypoint);
            newSquare.name = "Square_" + i;

            // Applique la couleur
            Material mat = squareMaterials[colorIndex % squareMaterials.Length];
            MeshRenderer renderer = newSquare.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.material = mat;

            if ((i + 1) % blockSize == 0) colorIndex++;

            // Ajoute le numéro (sauf pour la case 0)
            if (i > 0)
            {
                GameObject numberObj = Instantiate(numberPrefab, waypoint.position + numberOffset, Quaternion.identity);
                numberObj.transform.SetParent(newSquare.transform);
                numberObj.name = "Number_" + i;
                
                TextMeshPro tmp = numberObj.GetComponent<TextMeshPro>();
                if (tmp != null)
                    tmp.text = i.ToString(); // met le numéro
            }
        }

        Debug.Log("Plateau généré avec numéros !");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BoardBuilder))]
public class BoardBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BoardBuilder builder = (BoardBuilder)target;
        if (GUILayout.Button("Build Board"))
            builder.BuildBoard();
    }
}
#endif
