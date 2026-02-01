using UnityEngine;

public class Telephone : MonoBehaviour
{
    public PanningCamera panningCamera;
    public Animator DeskAnimator;

    void Start()
    {
        panningCamera.lookingChanged.AddListener(OnLookingChanged);
    }

    public void OnLookingChanged(PanningCamera.LookingAt lookingAt)
    {
        if (lookingAt == PanningCamera.LookingAt.Phone)
        {
            
        }
    }

    public void StartRinging()
    {
        DeskAnimator.Play("Telephone Ring");
    }

    public void StartCall()
    {
        DeskAnimator.StopPlayback();
    }
}
