using System;
using System.Collections.Generic;
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

    public List<GameMission> GameMissions;

    int index = 0;

    int errorScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCurrentMission();
    }

    void StartNextMission()
    {
        index += 1;
        if (index < GameMissions.Count)
        {
            StartCurrentMission();
        }
    }

    void StartCurrentMission()
    {
        telephone.GameMission = GameMissions[index];
        telephone.StartRinging();
    }


}
