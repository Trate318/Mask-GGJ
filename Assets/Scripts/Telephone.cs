using UnityEngine;

public class Telephone : MonoBehaviour
{
    public AK.Wwise.Event TelephoneRing;
    public PanningCamera panningCamera;
    public Animator DeskAnimator;
    public GameMission GameMission;

    void Start()
    {
        panningCamera.lookingChanged.AddListener(OnLookingChanged);
    }

    public void OnLookingChanged(PanningCamera.LookingAt lookingAt)
    {
        if (lookingAt == PanningCamera.LookingAt.Phone)
        {
            StartCall();
        }
    }

    public void StartRinging()
    {
        DeskAnimator.Play("Telephone Ring");
        TelephoneRing.Post(gameObject);
    }

    public void StartCall()
    {
        Debug.Log("HERE!!!!");
        DeskAnimator.Play("Idle");

        if (GameMission != null)
        {
            GameMission.BossCallEvent.Post(gameObject);
        }
    }
}
