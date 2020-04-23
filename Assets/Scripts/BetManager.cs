using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class BetManager : MonoBehaviour
{

    public float LagDelay;

    public static void GetABet(string incomingBet, string incomingBettersName, GameObject raceManagerGameObjectRef)
    {
        //print("We got a bet: "+ incomingBet);

        RaceManager racemanagerScriptRef = raceManagerGameObjectRef.GetComponent<RaceManager>();
        foreach (GameObject eachTurtle in racemanagerScriptRef.TurtlesInTheRace)
        {
            TurtleAI turtleScriptRef = eachTurtle.GetComponent<TurtleAI>();
            if (incomingBet.CaseInsensitiveContains("register"))
            {
                foreach (GuestData gD in GuestManager.AllGuests)
                {
                    if (gD.guestName == incomingBettersName)
                    {
                        gD.registeredAddress = incomingBet;
                    }
                }
            }
            if (incomingBet.CaseInsensitiveContains("bid"))
            {
                TurtleForSale turtleForBiddingScriptRef = racemanagerScriptRef.CurrentTurtlesForSale[0].GetComponent<TurtleForSale>();
                if (incomingBet.CaseInsensitiveContains(turtleForBiddingScriptRef.myName))
                {

                    //Debug.Log("incoming bet " + incomingBet);
                    turtleForBiddingScriptRef.GetABid(int.Parse(Regex.Match(incomingBet, @"\d+").Value), incomingBettersName);
                }
            }
            if (RaceManager.isBettingOpen)
            {
                string mightBeExclamation;
                if (incomingBet.CaseInsensitiveContains("!1")){

                }
                if (incomingBet.CaseInsensitiveContains(eachTurtle.name))
                {
                    //bet on the turtle
                    //print("Bet on this turtle " + eachTurtle.name);
                    DealWiththeBet(incomingBet, incomingBettersName, eachTurtle, turtleScriptRef, racemanagerScriptRef, raceManagerGameObjectRef);
                }
            }
            else
            {
                if (racemanagerScriptRef.hasRaceStarted && racemanagerScriptRef.hasRaceEnded !=true)
                {
                    if (incomingBet.CaseInsensitiveContains(eachTurtle.name))
                    {
                        TurtleAI tsRef = eachTurtle.GetComponent<TurtleAI>();
                        tsRef.BaseSpeed += 0.1f;
                        TwitchIRC tIRC = raceManagerGameObjectRef.GetComponent<TwitchIRC>();
                        string ConfirmedQuip = "Good luck!";
                        int RandomQuipNumber = Random.Range(0, 20);
                        switch (RandomQuipNumber)
                        {
                            case 0:
                                ConfirmedQuip = eachTurtle.name + " has recieved a boost of inspiration! PogChamp";
                                break;
                            case 1:
                                ConfirmedQuip = "Go " + eachTurtle.name + " Go! PogChamp";
                                break;
                            case 2:
                                ConfirmedQuip = eachTurtle.name + " is freaking out! PogChamp";
                                break;
                            case 3:
                                ConfirmedQuip = eachTurtle.name + " is going in strong! PogChamp";
                                break;
                            case 4:
                                ConfirmedQuip = eachTurtle.name + " got another wind! PogChamp";
                                break;
                            case 5:
                                ConfirmedQuip = eachTurtle.name + " is making a break for it! PogChamp";
                                break;
                            case 6:
                                ConfirmedQuip = eachTurtle.name + "hogs the spotlight! PogChamp";
                                break;
                            case 7:
                                ConfirmedQuip = eachTurtle.name + "! PogChamp";
                                break;
                            case 8:
                                ConfirmedQuip = "Excitement rippling around " + eachTurtle.name +" now! PogChamp";
                                break;
                            case 9:
                                ConfirmedQuip = eachTurtle.name + " for home! PogChamp";
                                break;
                            case 10:
                                ConfirmedQuip = eachTurtle.name + " giving chase! PogChamp";
                                break;
                            case 11:
                                ConfirmedQuip = eachTurtle.name + " stretches hard! PogChamp";
                                break;
                            case 12:
                                ConfirmedQuip = eachTurtle.name + " is under the whip! PogChamp";
                                break;
                            case 13:
                                ConfirmedQuip = eachTurtle.name + " around wide, this could be setup for something special! PogChamp";
                                break;
                            case 14:
                                ConfirmedQuip = "There's the run for " + eachTurtle.name +"! PogChamp";
                                break;
                            case 15:
                                ConfirmedQuip = eachTurtle.name + " hype! PogChamp";
                                break;
                            case 16:
                                ConfirmedQuip = eachTurtle.name + " takes footing in its stride! PogChamp";
                                break;
                            case 17:
                                ConfirmedQuip = "There's " + eachTurtle.name + " going home! PogChamp";
                                break;
                            case 18:
                                ConfirmedQuip = eachTurtle.name + " still has a little bit left in the tank! PogChamp";
                                break;
                            case 19:
                                ConfirmedQuip = eachTurtle.name + " is starting to wind up! PogChamp";
                                break;
                            case 20:
                                ConfirmedQuip = eachTurtle.name + " hears the crowd! PogChamp";
                                break;
                        }
                        tIRC.SendCommand("PRIVMSG #" + tIRC.channelName + " : " + ConfirmedQuip);
                        Debug.Log("Boosted");
                    }
                }
            }

        }

    }


    static void DealWiththeBet(string incomingBet, string incomingBettersName, GameObject eachTurtle,TurtleAI turtleScriptRef, RaceManager racemanagerScriptRef, GameObject raceManagerGameObjectRef)
    {
        if (incomingBet.CaseInsensitiveContains("win") || incomingBet.CaseInsensitiveContains("show") || incomingBet.CaseInsensitiveContains("place"))
        {
            //bet to win
            string numbersInMessage = Regex.Match(incomingBet, @"\d+").Value;
            int betAsInt = int.Parse(numbersInMessage);
            if (betAsInt < 0 || betAsInt > 1000000)
            {
                return;
            }
            BetData thisBet = new BetData();
            thisBet.TurtlesName = eachTurtle.name;
            thisBet.BetAmount = betAsInt;
            if (incomingBet.CaseInsensitiveContains("win"))
            {
                thisBet.BetType = "win";
                turtleScriptRef.howMuchIsBetOnMe += thisBet.BetAmount;
            }
            if (incomingBet.CaseInsensitiveContains("place"))
            {
                thisBet.BetType = "place";
                turtleScriptRef.howMuchIsBetOnMeToPlace += thisBet.BetAmount;
            }
            if (incomingBet.CaseInsensitiveContains("show"))
            {
                thisBet.BetType = "show";
                turtleScriptRef.howMuchIsBetOnMeToShow += thisBet.BetAmount;
            }
            thisBet.BettersName = incomingBettersName;
            thisBet.BetOdds = turtleScriptRef.myRealOdds;
            foreach (GuestData gD in GuestManager.AllGuests)
            {
                if (gD.guestName == thisBet.BettersName)
                {
                    if (gD.guestCash < thisBet.BetAmount)
                    {
                        thisBet.BetAmount = gD.guestCash;
                    }
                    if (gD.guestCash == thisBet.BetAmount)
                    {
                        GameObject bottomToaster = GameObject.Find("Toaster");
                        ToasterManager toastScriptRef = bottomToaster.GetComponent<ToasterManager>();
                        toastScriptRef.ShowAToaster(gD.guestName, " Went ALL IN!");
                    }
                    gD.guestCash -= thisBet.BetAmount;
                }
            }
            racemanagerScriptRef.CurrentRaceBets.Add(thisBet);

            TwitchIRC tIRC = raceManagerGameObjectRef.GetComponent<TwitchIRC>();
            if (!thisBet.BettersName.Contains("turtlebot"))
            {
                string betConfirmedQuip ="Good luck!";
                int RandomQuipNumber = Random.Range(0, 10);
                switch (RandomQuipNumber)
                {
                    case 0:
                        betConfirmedQuip = "Good luck!";
                        break;
                    case 1:
                        betConfirmedQuip = "Go Go Go!";
                        break;
                    case 2:
                        betConfirmedQuip = "Be sure to cheer it on!";
                        break;
                    case 3:
                        betConfirmedQuip = "It's a lock. KevinTurtle";
                        break;
                    case 4:
                        betConfirmedQuip = "It's been sandbagging looking for a good spot.";
                        break;
                    case 5:
                        betConfirmedQuip = "Ka-Chow!";
                        break;
                    case 6:
                        betConfirmedQuip = "They heard their going to break it's maiden. Kappa";
                        break;
                    case 7:
                        betConfirmedQuip = "It's workouts are unpublished. Kappa";
                        break;
                    case 8:
                        betConfirmedQuip = "Their UPS driver Lance knows it's owner. Kappa";
                        break;
                    case 9:
                        betConfirmedQuip = "It's been getting in light because they were using the bug boy on it. Now they're ready to run with it!";
                        break;
                    case 10:
                        betConfirmedQuip = "It's going to go for a great prize.";
                        break;
                }
                tIRC.SendCommand("PRIVMSG #" + tIRC.channelName + " : Bet confirmed. " + thisBet.BettersName + " Bet " + thisBet.BetAmount + " on " + thisBet.TurtlesName + " to " + thisBet.BetType +". " +betConfirmedQuip);

            }
        }
    }

    public static void FinalizeOdds(GameObject raceManagerGameObjectRef)
    {
        RaceManager racemanagerScriptRef = raceManagerGameObjectRef.GetComponent<RaceManager>();
        foreach (BetData betD in racemanagerScriptRef.CurrentRaceBets)
        {
            foreach (GameObject eachTurtle in racemanagerScriptRef.TurtlesInTheRace)
            {
                if (betD.TurtlesName == eachTurtle.name)
                {
                    TurtleAI turtleScriptRef = eachTurtle.GetComponent<TurtleAI>();
                    betD.BetOdds = turtleScriptRef.myRealOdds;
                }
            }
        }
    }



}
