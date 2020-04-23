using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class RaceManager : MonoBehaviour
{

    public GameObject[] TurtlesInTheRace, possibleTracks, possibleFinishLines;
    public List<BetData> CurrentRaceBets = new List<BetData>();
    //public TurtleData[] AllTurtlesData;
    public string TrackName;
    public TurtleForSale[] CurrentTurtlesForSale;
    public AudioClip bellSound;
    public GameObject RaceResultsScreen, InRaceresultsScreen, StartingLineLocator, FinishLineLocator, RaceStartingTimerObject, AuctionTimerObject, WholeAuctionObject, BettingClosedDialog, BettingOpenDialog, raceInfoDialog, RaceDetailsGridObject;
    public bool hasRaceStarted, hasRaceEnded, hasPlacedAIBets, hasAucionCompleted, startedMusic;
    public static bool isBettingOpen, isAuctionOpen;
    public float TimeBetweenRaces, timeRaceEnded;
    public int Purse = 1000;
    public CinemachineVirtualCamera BackHalfCam, FinishLineCam;
    public CinemachineDollyCart RaceCam1;
    public TextMeshProUGUI RaceResultsGUITextA, RaceResultsGUITextB, RaceResultsGuiTextC, RaceResultsGiuTextD, inRaceResultsGiuText, RaceStartingTimer, AuctionTimer, RaceStartingTitle, bettingOpenTitle, bettingOpenInstructions, RaceInfoSurface, RaceInfoLength;


    // Use this for initialization
    void Awake()
    {
        Debug.Log("Loading data...");
        GuestManager.LoadGuestData();
        //Debug.Log("Clearing race results...");
        GuestManager.CurrentRaceresults.Clear();
        TurtleAuctionManager.TurtlesForAuction.Clear();
        for (int i = 0; i <= 9; i++)
        {
            //TurtlesInTheRace[i].name = AllTurtlesData[i].name;
            if (GuestManager.NextRacesTurtles.Count <= i) //If there are less custom turtles entered than the number of names we have filled out
            {
                //Debug.Log("GuestManager.NextRacesTurtles.Count is at " + GuestManager.NextRacesTurtles.Count + " and i is at " + i);
                //print(i + " nextracesturtlescount " + GuestManager.NextRacesTurtles.Count);
                TurtlesInTheRace[i].name = gameObject.GetComponent<TurtleNamer>().GiveNewRandomName();
                TurtleAI thisAI = TurtlesInTheRace[i].GetComponent<TurtleAI>();
                thisAI.myTurtleData.MyScaleX = Random.Range(-10, 10);
                thisAI.myTurtleData.MyScaleY = Random.Range(-10, 10);
                //print("making a fake turtle name");
            }
            else
            {
                TurtlesInTheRace[i].GetComponent<TurtleAI>().myTurtleData = GuestManager.NextRacesTurtles[i];
            }
        }
        //Debug.Log("Deactivating objects...");
        DeactivateObjects(possibleTracks);
        DeactivateObjects(possibleFinishLines);

        int randomTrack = Random.Range(0, possibleTracks.Length -1);
        int randomLength = Random.Range(0, possibleFinishLines.Length -1);
        if (PlayerPrefs.HasKey("NextSurface"))
        {   
            randomTrack = PlayerPrefs.GetInt("NextSurface");
            randomLength = PlayerPrefs.GetInt("NextLength");
           //Debug.Log("Oh shit we loaded the random legnth from playerprefs, this has caused issues, it is " + randomLength);
        }
        if (PlayerPrefs.HasKey("NextPurse"))
        {
            Purse = PlayerPrefs.GetInt("NextPurse");
        }

        //Debug.Log("Picking random track...");
        possibleTracks[randomTrack].SetActive(true);
        //Debug.Log("Picking random finish line...");
        possibleFinishLines[randomLength].SetActive(true);
        //Debug.Log("Picking track name...");
        TrackName = possibleTracks[randomTrack].name;
        RaceInfoSurface.text = "Surface: " + TrackName;
        //Debug.Log("Writing track length...");
        RaceInfoLength.text = "Length: " + possibleFinishLines[randomLength].name;

        //Debug.Log("Setting up finish line...");
        FinishLineCam.gameObject.transform.position = new Vector3(FinishLineCam.transform.position.x, FinishLineCam.transform.position.y, possibleFinishLines[randomLength].transform.position.z);
        FinishLineLocator.transform.position = possibleFinishLines[randomLength].transform.position;

        isBettingOpen = true;
        //Debug.Log("picking random name for HUD...");
        string exampleName = TurtlesInTheRace[Random.Range(2, 5)].name;
        bettingOpenInstructions.text = "Type ''" + exampleName + " Win 10'' to bet 10 on " + exampleName + " to win!";
    }
    void DeactivateObjects(GameObject[] objectsToKill)
    {
        foreach (GameObject oTK in objectsToKill)
        {
            oTK.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > 1 && !hasPlacedAIBets)
        {
            TwitchChatExample ChatLink = gameObject.GetComponent<TwitchChatExample>();
            for (int i = 0; i <= 128; i++)
            {
                ChatLink.fakeMessage(TurtlesInTheRace[Random.Range(0, 10)].name);
                if (i > 122)
                { 
                   
                        ChatLink.fakeMessage("bid A " + CurrentTurtlesForSale[0].GetComponent<TurtleForSale>().myName + " " + Random.Range(1, 10));
                }

            }
            hasPlacedAIBets = true;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (BonusRoundManager.IsMusicEnabled)
            {
                BonusRoundManager.IsMusicEnabled = false;
                Debug.Log("Disabling Music");
            }
            else
            {
                BonusRoundManager.IsMusicEnabled = true;
                Debug.Log("Enabling Music");
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {

            foreach (GameObject eachTurtle in TurtlesInTheRace)
            {
                TurtleAI ti = eachTurtle.GetComponent<TurtleAI>();
                ti.BaseSpeed = 50;
            }
            TimeBetweenRaces = 2;
        }
        if (Input.GetKeyDown(KeyCode.S))
        { //start the race shortcut
            TimeBetweenRaces = 2;
        }

        if (TimeBetweenRaces - 9 < Time.timeSinceLevelLoad && BettingOpenDialog.activeInHierarchy == true)
        {
            AudioSource.PlayClipAtPoint(bellSound, Camera.main.transform.position);
            BettingClosedDialog.SetActive(true);
            BettingOpenDialog.SetActive(false);
            raceInfoDialog.SetActive(false);
            RaceDetailsGridObject.SetActive(false);
        }
        if(TimeBetweenRaces - Time.timeSinceLevelLoad< 40 &&!startedMusic)
        {
            print("time race ended " + timeRaceEnded + " time between races " + TimeBetweenRaces);

            MainCameraHelper.StartTheRaceMusic();
            startedMusic = true;
        }
        if (TimeBetweenRaces - 6 < Time.timeSinceLevelLoad && isBettingOpen)
        {
            isBettingOpen = false;
        }

        if (TimeBetweenRaces * 0.9f < Time.timeSinceLevelLoad && !hasAucionCompleted)
        {
            Auctioneer.FinalizeAuction(this.gameObject);
            hasAucionCompleted = true;
            AuctionTimerObject.SetActive(false);
            WholeAuctionObject.SetActive(false);
        }
        else
        {
            AuctionTimer.text = Mathf.RoundToInt((TimeBetweenRaces * 0.9f) - Time.timeSinceLevelLoad) + " Seconds";
        }

        if (TimeBetweenRaces < Time.timeSinceLevelLoad && !hasRaceStarted)
        {
            hasRaceStarted = true;
            RaceStartingTimerObject.SetActive(false);
            BettingClosedDialog.SetActive(false);
            RaceCam1.m_Speed = 1;
            //TimeBetweenRaces += Time.time; //this was causing extra delays
            BetManager.FinalizeOdds(this.gameObject);
        }
        else
        {
            RaceStartingTimer.text = Mathf.RoundToInt(TimeBetweenRaces - Time.timeSinceLevelLoad) + " Seconds";
        }
        if (hasRaceStarted && FinishLineCam.Priority != 16)
        {
            foreach (GameObject turtleRef in TurtlesInTheRace)
            {
                TurtleAI turlteScriptRef = turtleRef.GetComponent<TurtleAI>();
                /*if(BackHalfCam.Priority!=15 && turlteScriptRef.percentFinished > 0.5f){
					BackHalfCam.Priority = 15;
				}*/
                if (turlteScriptRef.percentFinished > 0.9f)
                {
                    FinishLineCam.Priority = 16;
                }
            }
        }

        //If a single turtle finshed, open the in-race results gui.
        if(TurtleAI.HowManyTurtlesFinished > 0 && !hasRaceEnded)
        {
            if (!InRaceresultsScreen.activeInHierarchy)
            {
                InRaceresultsScreen.SetActive(true);
            }
        }

        if (TurtleAI.HowManyTurtlesFinished == 10 && !hasRaceEnded)
        {
            hasRaceEnded = true;
            PayOutWinners();
            InRaceresultsScreen.SetActive(false);
            RaceResultsScreen.SetActive(true);
            RaceStartingTitle.text = "Bonus Round in";
            BettingClosedDialog.SetActive(true);
            raceInfoDialog.SetActive(true);
            RaceResultsGUITextA.text = TurtleAI.RaceResultsColumn1 + "<nobr>";
            RaceResultsGUITextB.text = TurtleAI.RaceResultsColumn2 + "<nobr>";
            RaceResultsGuiTextC.text = TurtleAI.RaceResultsColumn3 + "<nobr>";
            RaceResultsGiuTextD.text = TurtleAI.RaceResultsColumn4 + "<nobr>";

            timeRaceEnded = Time.timeSinceLevelLoad;
            int totalTrueBets = 0;
            float totalPaidOut = 0;
            foreach (BetData eachBet in CurrentRaceBets)
            {


                //Check for winning bets
                if (eachBet.BetType == "win")
                {
                    foreach (RaceResultData contestant in GuestManager.CurrentRaceresults)
                    {
                        if (contestant.FinishingPlace == 1 && contestant.TurtleName == eachBet.TurtlesName)
                        {
                            //They predicted this win!
                            eachBet.didBetComeTrue = true;
                            totalTrueBets++;
                        }
                    }
                }
                if (eachBet.BetType == "place")
                {
                    foreach (RaceResultData contestant in GuestManager.CurrentRaceresults)
                    {
                        if (contestant.TurtleName == eachBet.TurtlesName)
                        {
                            if (contestant.FinishingPlace == 1 || contestant.FinishingPlace == 2)
                            {
                                eachBet.didBetComeTrue = true;
                                totalTrueBets++;
                            }
                        }
                    }
                }
                if (eachBet.BetType == "show")
                {
                    foreach (RaceResultData contestant in GuestManager.CurrentRaceresults)
                    {
                        if (contestant.TurtleName == eachBet.TurtlesName)
                        {
                            if (contestant.FinishingPlace <= 3)
                            {
                                eachBet.didBetComeTrue = true;
                                totalTrueBets++;
                            }
                        }
                    }
                }

                foreach (GuestData eachGuest in GuestManager.AllGuests)
                {

                    if (eachGuest.guestName == eachBet.BettersName)
                    {
                        if (eachBet.didBetComeTrue)
                        {
                            //Pay them if the bet won.
                            float amountToPay = eachBet.BetAmount * eachBet.BetOdds;
                            TwitchIRC tIRC = GetComponent<TwitchIRC>();
                            if (eachBet.BetType == "place")
                            {
                                amountToPay = eachBet.BetAmount + OddsDisplay.CurrentTotalPlacePool * (eachBet.BetAmount / (OddsDisplay.CurrentTotalPlacePool / 2));
                                //Debug.Log("Paying out " + amountToPay + " to " + eachGuest.guestName + " for betting " + eachBet.BetAmount + " on " + eachBet.TurtlesName + " to place of a pool of " + OddsDisplay.CurrentTotalPlacePool);
                                if (!eachGuest.guestName.Contains("turtlebot"))
                                {
                                    //tIRC.SendMsg("Paying out " + amountToPay + " to " + eachGuest.guestName + " for betting " + eachBet.BetAmount + " on " + eachBet.TurtlesName + "to place of a pool of " + OddsDisplay.CurrentTotalPlacePool);
                                }
                            }
                            if (eachBet.BetType == "win")
                            {
                                //Debug.Log("Paying out " + amountToPay + " to " + eachGuest.guestName + " for betting " + eachBet.BetAmount + " on " + eachBet.TurtlesName + " at odds of " + eachBet.BetOdds);
                                if (!eachGuest.guestName.Contains("turtlebot"))
                                {
                                    //tIRC.SendMsg("Paying out " + amountToPay + " to " + eachGuest.guestName + " for betting " + eachBet.BetAmount + " on " + eachBet.TurtlesName + " at odds of " + eachBet.BetOdds);
                                }
                            }
                            if (eachBet.BetType == "show")
                            {
                                amountToPay = eachBet.BetAmount + OddsDisplay.CurrentTotalShowPool * (eachBet.BetAmount / (OddsDisplay.CurrentTotalShowPool / 3));
                                //Debug.Log("Paying out " + amountToPay + " to " + eachGuest.guestName + " for betting " + eachBet.BetAmount + " on " + eachBet.TurtlesName + " to show of a pool of " + OddsDisplay.CurrentTotalShowPool);
                                if (!eachGuest.guestName.Contains("turtlebot"))
                                {
                                    //tIRC.SendMsg("Paying out " + amountToPay + " to " + eachGuest.guestName + " for betting " + eachBet.BetAmount + " on " + eachBet.TurtlesName + " to show of a pool of " + OddsDisplay.CurrentTotalShowPool);
                                }
                            }
                            eachGuest.guestCash += amountToPay;
                            totalPaidOut += amountToPay;
                            float totalPaidIn = OddsDisplay.CurrentTotalPlacePool + OddsDisplay.CurrentTotalPot + OddsDisplay.CurrentTotalShowPool;
                            //print("Total paid out = " + totalPaidOut + " Total paid in = " + totalPaidIn);





                        }
                    }

                }

            }

        }
        if (timeRaceEnded > 1)
        {
            if (RaceStartingTimerObject.activeInHierarchy != true)
            {
                RaceStartingTimerObject.SetActive(true);
            }
            RaceStartingTimer.text = Mathf.RoundToInt((timeRaceEnded + (TimeBetweenRaces / 5)) - Time.timeSinceLevelLoad) + " Seconds";
            RaceStartingTitle.text = "Bonus Round in...";
            //Debug.Log("Time race ended " + timeRaceEnded + "  Time between races " + TimeBetweenRaces + "  current time " + Time.time);
            if (Time.timeSinceLevelLoad > timeRaceEnded + (TimeBetweenRaces / 5))
            {
                GuestManager.SaveGuestData();
                GuestManager.ClearNextRaceTurtles();
                SceneManager.LoadScene("Pachinko");

            }
        }
    }

    void PayOutWinners()
    {
        foreach(RaceResultData contestant in GuestManager.CurrentRaceresults)
        {
            foreach(GuestData possibleOwner in GuestManager.AllGuests)
            {
                if(contestant.OwnerName == possibleOwner.guestName)
                {
                    float reward = 0;
                    if(contestant.FinishingPlace == 1)
                    {
                        reward = Purse * 0.6f;
                    }
                    if (contestant.FinishingPlace == 2)
                    {
                        reward = Purse * 0.2f;
                    }
                    if (contestant.FinishingPlace == 1)
                    {
                        reward= Purse * 0.11f;
                    }
                    if (contestant.FinishingPlace == 1)
                    {
                        reward = Purse * 0.06f;
                    }
                    if (contestant.FinishingPlace == 1)
                    {
                        reward = Purse * 0.03f;
                    }
                    possibleOwner.guestCash += reward;
                    //print(reward + " given to " + contestant.OwnerName + " for their turtle finishing " + contestant.FinishingPlace);
                }
            }
        }
    }
}
