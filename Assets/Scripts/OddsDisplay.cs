using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
public class OddsDisplay : MonoBehaviour
{

    // Use this for initialization

    public CinemachineVirtualCamera PreRaceCam;
    public RaceManager TheRaceManager;
    public TextMeshProUGUI WinPool, ShowPool, PlacePool;

    public static float CurrentTotalPot, CurrentTotalShowPool, CurrentTotalPlacePool;
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GuestManager.SaveGuestData();
        }
        if (TheRaceManager.hasRaceStarted)
        {
            gameObject.SetActive(false);
            PreRaceCam.Priority = 0;
        }
        CurrentTotalPot = 0;
        CurrentTotalShowPool = 0;
        CurrentTotalPlacePool = 0;
		RaceManager racemanagerScript = TheRaceManager.GetComponent<RaceManager>();
        foreach (BetData bD in racemanagerScript.CurrentRaceBets)
        {
            if(bD.BetType=="win"){
                CurrentTotalPot += bD.BetAmount;
                WinPool.text = CurrentTotalPot.ToString();
            }
            if(bD.BetType=="show"){
                CurrentTotalShowPool+=bD.BetAmount;
                ShowPool.text = CurrentTotalShowPool.ToString();
            }
            if(bD.BetType=="place"){
                CurrentTotalPlacePool+=bD.BetAmount;
                PlacePool.text = CurrentTotalPlacePool.ToString();
            }
        }


    }
}
