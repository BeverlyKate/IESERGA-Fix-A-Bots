using UnityEngine;

public class WireSlot : MonoBehaviour
{
    public Material defaultMaterial;  // 🔵 Blue (empty slot)
    public Material correctMaterial;  // 🟢 Green (correct wire)
    public Material incorrectMaterial; // 🔴 Red (wrong wire)

    private MeshRenderer meshRenderer;
    public bool isOccupied = false;

    public bool isCorrect = false;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError($"❌ ERROR: No MeshRenderer found on {gameObject.name}!");
        }
        ResetSlot(); // Set default color
    }

    public void SetOccupied(bool occupied, bool correctWire)
    {
        isOccupied = occupied;

        isCorrect = correctWire;
        if (meshRenderer != null)
        {
            Material materialInstance = meshRenderer.material; // ✅ Creates a unique material instance

            if (!occupied)
            {
                materialInstance.SetColor("_BaseColor", Color.blue); // 🔵 Empty slot
                Debug.Log($"🔵 Slot {gameObject.name} is now EMPTY and BLUE.");
            }
            else if (correctWire)
            {
                materialInstance.SetColor("_BaseColor", Color.green); // 🟢 Correct wire
                Debug.Log($"🟢 Slot {gameObject.name} is now GREEN (Correct Wire).");
            }
            else
            {
                materialInstance.SetColor("_BaseColor", Color.red); // 🔴 Incorrect wire
                Debug.Log($"🔴 Slot {gameObject.name} is now RED (Incorrect Wire).");
            }

            meshRenderer.material = materialInstance; // ✅ Force apply the material instance
        }
        else
        {
            Debug.LogError($"❌ ERROR: MeshRenderer missing on {gameObject.name}");
        }
    }

    public void ResetSlot()
    {
        // ✅ Only reset if not occupied (prevents instant reset)
        if (!isOccupied)
        {
            SetOccupied(false, false);
        }
    }

    
}