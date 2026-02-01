using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameMission", order = 1)]
public class GameMission : ScriptableObject
{
    public AK.Wwise.Event BossCallEvent;
    public AK.Wwise.Event AudioClip;
    public String BossCallEventDescription;
    public String AudioClipDescription;
}

public class MissionManager : MonoBehaviour
{
    public Telephone telephone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        telephone.StartRinging();
    }

}
