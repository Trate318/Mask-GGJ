using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public Telephone telephone;

    public GameObject MainScreen;
    public GameObject GameScreen;

    public List<GameMission> GameMissions;

    int index = 0;

    int errorScore = 0;

    public void StartNextMission()
    {
        index += 1;
        if (index < GameMissions.Count)
        {
            StartCurrentMission();
        }
    }

    public void StartCurrentMission()
    {
        telephone.GameMission = GameMissions[index];
        telephone.StartRinging();

        MainScreen.SetActive(false);
        GameScreen.SetActive(true);
    }


}
