using UnityEngine;

public enum WireColor3 { Red, Yellow }

public class Wire3 : MonoBehaviour
{
    public WireColor3 wireColor3; // Assign in Inspector
    private WireStation3 wireStation3;
    public Transform currentSlot;
    public float defaultZ; // Stores the original Z position
    private int? touchId = null; // Track which finger is interacting with this wire

    private void Start()
    {
        wireStation3 = FindObjectOfType<WireStation3>();
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
                        wireStation3.EndTouch(this);
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
        wireStation3.StartDragging(this);
    }

    public int GetTouchId()
    {
        return touchId ?? -1;
    }
}