using UnityEngine;

public class WireSlot : MonoBehaviour
{
    public Material defaultMaterial;  // Assign in Inspector (default slot color)
    public Material occupiedMaterial; // Assign in Inspector (color when wire is placed)
    private MeshRenderer meshRenderer;
    public bool isOccupied = false; // Track if a wire is already placed in this slot

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ResetSlot(); // Set default color
    }

    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        if (meshRenderer != null)
        {
            meshRenderer.material = occupied ? occupiedMaterial : defaultMaterial;
        }
    }

    public void ResetSlot()
    {
        SetOccupied(false);
    }
}
