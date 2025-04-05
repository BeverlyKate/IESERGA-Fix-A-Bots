using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WireStation3 : MonoBehaviour
{
    private Wire3 draggedWire = null;
    private Vector3 offset;
    public List<Wire3> wires = new List<Wire3>();
    public Transform[] wireSlots; // Assign in the Inspector
    public GameObject correctIndicator; // Assign a visual indicator
    public Camera mainCamera; // Assign in Inspector!

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
            // Find the touch that's dragging this wire
            Touch? activeTouch = null;
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == draggedWire.GetTouchId())
                {
                    activeTouch = touch;
                    break;
                }
            }

            if (activeTouch.HasValue)
            {
                Ray ray = mainCamera.ScreenPointToRay(activeTouch.Value.position);
                Plane dragPlane = new Plane(Vector3.up, draggedWire.transform.position);
                float distance;

                if (dragPlane.Raycast(ray, out distance))
                {
                    Vector3 worldPosition = ray.GetPoint(distance);
                    draggedWire.transform.position = new Vector3(worldPosition.x + offset.x, draggedWire.transform.position.y, worldPosition.z + offset.z);
                }
            }
            else
            {
                // Touch was lost, drop the wire
                DropWire(draggedWire);
                draggedWire = null;
            }
        }
        else
        {
            // Check for new touches on wires
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Wire3 wire = hit.collider.GetComponent<Wire3>();
                        if (wire != null)
                        {
                            wire.StartTouch(touch.fingerId);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void EndTouch(Wire3 wire)
    {
        if (draggedWire == wire)
        {
            DropWire(wire);
            draggedWire = null;
        }
    }

    public void StartDragging(Wire3 wire)
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

        // Raycast to find initial touch position in world space
        Touch? touch = null;
        foreach (Touch t in Input.touches)
        {
            if (t.fingerId == wire.GetTouchId())
            {
                touch = t;
                break;
            }
        }

        if (touch.HasValue)
        {
            Ray ray = mainCamera.ScreenPointToRay(touch.Value.position);
            Plane dragPlane = new Plane(Vector3.up, wire.transform.position);
            float distance;

            if (dragPlane.Raycast(ray, out distance))
            {
                Vector3 worldPosition = ray.GetPoint(distance);
                offset = wire.transform.position - worldPosition;
            }
        }
    }

    private void DropWire(Wire3 wire)
    {
        wire.transform.localScale = Vector3.one;

        float snapRange = 1.0f;
        Transform closestSlot = null;
        WireSlot closestWireSlot = null;
        float minDistance = float.MaxValue;

        // Find the touch position for this wire
        Vector2 touchPosition = Vector2.zero;
        bool foundTouch = false;
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == wire.GetTouchId())
            {
                touchPosition = touch.position;
                foundTouch = true;
                break;
            }
        }

        if (!foundTouch) return;

        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
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

        if (closestSlot != null)
        {
            wire.currentSlot = closestSlot;
            bool isCorrect = CheckIfCorrectWire(closestSlot, wire);
            closestWireSlot.SetOccupied(true, isCorrect);
            closestWireSlot.isOccupied = true;
        }

        CheckCorrectOrder();
    }

    private void CheckCorrectOrder()
    {
        // Ensure all slots are occupied before checking order
        if (wires.Count != wireSlots.Length)
        {
            correctIndicator.SetActive(false);
            ResetAllSlots();
            return;
        }

        // Sort wires based on X position (left to right)
        wires.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // Check if the exact order is Yellow → Red → Red → Red
        if (wires.Count == 4 &&
            wires[0].wireColor3 == WireColor3.Yellow &&
            wires[1].wireColor3 == WireColor3.Red &&
            wires[2].wireColor3 == WireColor3.Red &&
            wires[3].wireColor3 == WireColor3.Red)
        {
            correctIndicator.SetActive(true);
            GetComponentInParent<WiringTeleport>().TriggerTeleport();
        }
        else
        {
            correctIndicator.SetActive(false);
            ResetAllSlots();
        }
    }

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

    private bool CheckIfCorrectWire(Transform slot, Wire3 wire)
    {
        int slotIndex = System.Array.IndexOf(wireSlots, slot);
        WireColor3[] expectedColors = { WireColor3.Red, WireColor3.Yellow, WireColor3.Red, WireColor3.Red };

        if (slotIndex >= 0 && slotIndex < expectedColors.Length)
        {
            bool isCorrect = wire.wireColor3 == expectedColors[slotIndex];
            if (isCorrect)
            {
                correctIndicator.SetActive(true);
            }
            return isCorrect;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (draggedWire != null)
        {
            float snapRange = 1.0f;
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(draggedWire.transform.position, Vector3.up, snapRange);
        }
    }
#endif
}