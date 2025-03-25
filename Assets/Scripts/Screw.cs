using UnityEngine;

public class Screw : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void rotateScrew()
    {
        transform.eulerAngles = new Vector3(transform.position.x, transform.position.y + 22.5f, transform.position.z);
    }
}
