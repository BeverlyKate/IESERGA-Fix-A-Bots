using UnityEngine;

public enum WireColor { Red, Yellow }

public class Wire : MonoBehaviour
{
    public WireColor wireColor; // Assign in Inspector
    private WireStation wireStation;
    public Transform currentSlot;
    public float defaultZ; // Stores the original Z position
    private int? touchId = null; // Track which finger is interacting with this wire

    private void Start()
    {
        wireStation = FindObjectOfType<WireStation>();
        defaultZ = transform.position.z; // Save original Z position
    }

    private void Update()
    {
        // Handle touch input
        if (touchId.HasValue)
        {
            bool touchStillActive = false;
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == touchId.Value)
                {
                    touchStillActive = true;
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        wireStation.EndTouch(this);
                        touchId = null;
                    }
                    break;
                }
            }

            if (!touchStillActive)
            {
                touchId = null;
            }
        }
    }

    public void StartTouch(int fingerId)
    {
        touchId = fingerId;
        wireStation.StartDragging(this);
    }
}