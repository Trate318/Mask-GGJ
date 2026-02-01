using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Telephone telephone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        telephone.StartRinging();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
