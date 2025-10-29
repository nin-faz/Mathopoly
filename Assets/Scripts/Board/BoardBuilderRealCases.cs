using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CaseInfo
{
    public string name;      // clé (ex: "Case_Default", "Case_Prison", ...)
    public GameObject prefab; // le prefab correspondant
}

public class BoardBuilderRealCases : MonoBehaviour
{
    [Header("Référence vers le parent des waypoints (Waypoints)")]
    public Transform waypointsParent;

    [Header("Liste des prefabs disponibles (nom clé doit correspondre)")]
    public List<CaseInfo> allCases = new List<CaseInfo>();

    [Header("Options")]
    public bool skipWaypointZero = true; // si true, on n'écrase pas waypoint_0 (départ)
    public bool verbose = true;

    // Exécuter depuis l'inspecteur via le menu (ContextMenu) ou appeler ReplaceAllCases() par script
    [ContextMenu("Replace All Cases In Waypoints")]
    public void ReplaceAllCases()
    {
        if (waypointsParent == null)
        {
            Debug.LogError("[BoardBuilderRealCases] waypointsParent non assigné.");
            return;
        }

        int total = waypointsParent.childCount;
        if (verbose) Debug.Log($"[BoardBuilderRealCases] Traitement de {total} waypoints...");

        for (int i = 0; i < total; i++)
        {
            Transform waypoint = waypointsParent.GetChild(i);
            if (waypoint == null) continue;

            if (skipWaypointZero && i == 0)
            {
                if (verbose) Debug.Log($"[BoardBuilderRealCases] Skip waypoint_0 (index 0)");
                continue;
            }

            // On s'attend à trouver Square_i dans le waypoint
            string squareName = $"Square_{i}";
            Transform square = waypoint.Find(squareName);
            if (square == null)
            {
                Debug.LogWarning($"[BoardBuilderRealCases] {squareName} introuvable dans {waypoint.name} (index {i}).");
                continue;
            }

            // On cherche la case placeholder nommée Case_i
            string caseName = $"Case_{i}";
            Transform oldCase = square.Find(caseName);
            if (oldCase == null)
            {
                Debug.LogWarning($"[BoardBuilderRealCases] {caseName} introuvable dans {square.name}.");
                continue;
            }

            // Memoriser transform local pour replacer la nouvelle case
            Vector3 localPos = oldCase.localPosition;
            Quaternion localRot = oldCase.localRotation;
            Vector3 localScale = oldCase.localScale;

            // Déterminer quel type de case on veut pour cet index
            string caseTypeKey = GetCaseType(i);
            GameObject prefab = FindCasePrefab(caseTypeKey);

            if (prefab == null)
            {
                Debug.LogWarning($"[BoardBuilderRealCases] Aucun prefab trouvé pour la clé '{caseTypeKey}' (index {i}). On laisse la case existante.");
                continue; // on ne détruit pas l'ancienne si on n'a pas de prefab correspondant
            }

            // Instancie le prefab comme enfant de square
            GameObject newCase = Instantiate(prefab, square);
            
            newCase.transform.localPosition = localPos;
            newCase.transform.localScale = localScale;

            // Renommer la nouvelle case avec le numéro (ex: Case_Tour_Sauté_20)
            newCase.name = $"{caseTypeKey}_{i}";

            // Supprimer l'ancienne case placeholder
            DestroyImmediate(oldCase.gameObject);

            if (verbose) Debug.Log($"[BoardBuilderRealCases] Remplacé {caseName} par {newCase.name}");
        }

        if (verbose) Debug.Log("[BoardBuilderRealCases] Remplacement terminé.");
    }

    private bool hasWarnedFallback = false;
    // ---- Logique pour décider quel type de case placer à l'index donné
    // Tu peux remplacer/adapter complètement cette fonction pour correspondre à ton plateau réel.
    private string lastCategory = ""; // mémorise la catégorie précédente

    private string GetCaseType(int index)
    {
        int lastIndex = waypointsParent.childCount - 1;
        if (index == 0) return "Case_Default";
        if (index == lastIndex) return "Case_Final";

        // Cas spéciaux fixes
        if (index == 23 || index == 35 || index == 49 || index == 60) return "Case_Teleportation";
        if (index == 15 || index == 22 || index == 40 || index == 65 || index == 81) return "Case_Stop";
        if (index == 18 || index == 37 || index == 58 || index == 71 || index == 88) return "Case_Prison";

        string chosenType = "Case_Default";
        string category = "";

        int safety = 0; // sécurité anti-boucle infinie
        do
        {
            float rnd = Random.value;
            if (rnd < 0.45f)
            {
                float q = Random.value;
                if (q < 0.33f) chosenType = "Case_Question_Blue";
                else if (q < 0.66f) chosenType = "Case_Question_Pink";
                else chosenType = "Case_Question_Green";
                category = "question";
            }
            else if (rnd < 0.65f)
            {
                chosenType = "Case_Default";
                category = "default";
            }
            else if (rnd < 0.75f)
            {
                chosenType = (Random.value < 0.5f) ? "Case_Bonus_Money" : "Case_Bonus_Move";
                category = "bonus";
            }
            else if (rnd < 0.85f)
            {
                chosenType = (Random.value < 0.5f) ? "Case_Malus_Money" : "Case_Malus_Move";
                category = "malus";
            }
            else
            {
                chosenType = "Case_Default";
                category = "default";
            }

            safety++;
            if (safety > 20) break; // évite blocage si logique cassée
        }
        while (category == lastCategory); // ⚠️ empêche deux catégories identiques de suite

        lastCategory = category;
        return chosenType;
    }

    // ---- Cherche le prefab correspondant dans la liste allCases par clé 'name'
    private GameObject FindCasePrefab(string key)
    {
        for (int i = 0; i < allCases.Count; i++)
        {
            if (allCases[i].name == key)
                return allCases[i].prefab;
        }
        return null;
    }
}
