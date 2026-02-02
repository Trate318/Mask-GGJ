using UnityEngine;

public class AudioEditHandler : MonoBehaviour
{
    public GameMission GameMission;

    public AK.Wwise.Event PauseEvent;

    public void Setup()
    {
        // image.sprite = GameMission.AudioClipTexture;
    }

    public void Update()
    {
    
    } 

    public void PlayTrack()
    {
        GameMission.AudioClip.Post(gameObject);
    }

    public void PauseTrack()
    {

    }
}
