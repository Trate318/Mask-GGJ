
using UnityEngine;

public class ScreenSaver : MonoBehaviour
{
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(Random.Range(0, 1f), Random.Range(0, 1f)).normalized * 30;
    }
    
}
