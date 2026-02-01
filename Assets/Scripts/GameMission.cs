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