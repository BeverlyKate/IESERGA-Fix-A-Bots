using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WireStation : MonoBehaviour
{
    private Wire draggedWire = null;
    private Vector3 offset;
    public List<Wire> wires = new List<Wire>();
    public Transform[] wireSlots; // Assign in the Inspector
    public GameObject correctIndicator; // Assign a visual indicator

    public Camera mainCamera; // Assign in Inspector!

    //public Transform targetObject; // Assign in Inspector
    public float yOffset = 1.0f; // Adjust in Inspector

    private bool isDone = false;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogError("No Camera assigned or found in the scene!");
        }
    }

    private void Update()
    {
        if (draggedWire != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane dragPlane = new Plane(Vector3.up, draggedWire.transform.position); // Plane aligned with the wire's height
            float distance;

            if (dragPlane.Raycast(ray, out distance))
            {
                Vector3 worldPosition = ray.GetPoint(distance);
                draggedWire.transform.position = new Vector3(worldPosition.x + offset.x, draggedWire.transform.position.y, worldPosition.z + offset.z);
            }

            if (Input.GetMouseButtonUp(0)) // Release the wire
            {
                DropWire(draggedWire);
                draggedWire = null;
            }
        }
    }

    public void StartDragging(Wire wire)
    {
        draggedWire = wire;

        // Free up the slot before dragging
        if (wire.currentSlot != null)
        {
            WireSlot previousSlot = wire.currentSlot.GetComponent<WireSlot>();
            if (previousSlot != null)
            {
                previousSlot.SetOccupied(false, false);
            }
            wire.currentSlot = null;
        }

        // Raycast to find initial mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, wire.transform.position);
        float distance;

        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            offset = wire.transform.position - worldPosition;
        }
    }

    private void DropWire(Wire wire)
{
    wire.transform.localScale = Vector3.one; // Ensure scale remains unchanged

    float snapRange = 1.0f; // Detection range
    Transform closestSlot = null;
    WireSlot closestWireSlot = null;
    float minDistance = float.MaxValue;

    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

    foreach (RaycastHit hit in hits)
    {
        WireSlot wireSlotComponent = hit.collider.GetComponent<WireSlot>();

        if (wireSlotComponent != null)
        {
            float distance = Vector3.Distance(wire.transform.position, hit.point);
            if (distance < minDistance && distance < snapRange)
            {
                closestSlot = hit.collider.transform;
                closestWireSlot = wireSlotComponent;
                minDistance = distance;
            }
        }
    }

    // If a nearby slot is found, check if the wire is correct
    if (closestSlot != null)
    {
        wire.currentSlot = closestSlot;

        // Check if the wire matches the correct pattern
        bool isCorrect = CheckIfCorrectWire(closestSlot, wire);

        // ✅ Change color based on correctness (ensuring persistence)
        closestWireSlot.SetOccupied(true, isCorrect); 

        Debug.Log($"🟢 Wire {wire.name} placed near slot {closestSlot.name} (Correct: {isCorrect})");
        Debug.Log($"🔴 Slot {closestSlot.name} should be {(isCorrect ? "Green (Correct)" : "Red (Incorrect)")}");

        // ✅ Prevent ResetSlot() from overwriting the color
        closestWireSlot.isOccupied = true;
    }
    else
    {
        Debug.LogWarning($"⚠️ Wire {wire.name} was dropped but not near any slot!");
    }

    CheckCorrectOrder();
}




    private void CheckCorrectOrder()
{
    bool allCorrect = true;

    foreach (Transform slot in wireSlots)
    {
        WireSlot wireSlot = slot.GetComponent<WireSlot>();

        if (wireSlot == null || !wireSlot.isCorrect)
        {
            allCorrect = false;
            break;
        }
    }

    if (allCorrect)
    {
        correctIndicator.SetActive(true);
        isDone = true;
        //TeleportWiresToTarget(); // ✅ Move wires when all slots are correct
    }
    else
    {
        correctIndicator.SetActive(false);
    }
}


    // Reset all wire slots to default material
    private void ResetAllSlots()
    {
        foreach (Transform slot in wireSlots)
        {
            WireSlot wireSlot = slot.GetComponent<WireSlot>();
            if (wireSlot != null)
            {
                wireSlot.ResetSlot();
            }
        }
    }

    public bool GameIsDone()
    {
        if (isDone)
        {
            return true;
        }
        return false;
    }

    /*
    private void TeleportWiresToTarget()
{
    if (targetObject == null)
    {
        Debug.LogWarning("⚠️ Target Object is not assigned!");
        return;
    }

    float spacing = 0.5f; // Adjust if needed
    Vector3 startPosition = targetObject.position - new Vector3((wires.Count - 1) * spacing * 0.5f, 0f, 0f);

    for (int i = 0; i < wires.Count; i++)
    {
        Vector3 newPosition = new Vector3(
            startPosition.x + i * spacing,
            targetObject.position.y + yOffset,
            targetObject.position.z
        );

        wires[i].transform.position = newPosition;
    }

    Debug.Log("✅ All wires teleported to target!");
}
    */

    private bool CheckIfCorrectWire(Transform slot, Wire wire)
    {
        int slotIndex = System.Array.IndexOf(wireSlots, slot);

        // Expected wire colors in order: Yellow, Red, Red, Red
        WireColor[] expectedColors = { WireColor.Yellow, WireColor.Red, WireColor.Red, WireColor.Red };

        if (slotIndex >= 0 && slotIndex < expectedColors.Length)
        {
            return wire.wireColor == expectedColors[slotIndex]; // ✅ Returns true if wire matches expected color
        }

        return false;
    }

    // 🟢 **Properly Place `OnDrawGizmos()` Here**
#if UNITY_EDITOR
private void OnDrawGizmos()
{
    Debug.Log("🟡 OnDrawGizmos is running!");
        Debug.Log($"🟡 OnDrawGizmos running! draggedWire = {(draggedWire != null ? draggedWire.name : "null")}");

    if (draggedWire != null)
    {
        float snapRange = 1.0f; // Adjust snap detection range

        // 🔵 Draw a transparent wire disc around the wire to show snap area
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(draggedWire.transform.position, Vector3.up, snapRange);
    }
}
#endif

}