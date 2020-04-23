using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(TwitchIRC))]
public class BonusRoundManager : MonoBehaviour
{
    public static bool IsMusicEnabled = true;
    private TwitchIRC IRC;
    public float xposition;
    public string movedirection;
    public float SpeedMultiplier = 5, lengthOfGame = 60;
    public GameObject ballToSpawn;
    public TextMeshProUGUI RaceStartingTimer, NextRaceSurfaceHud, NextRaceLengthHud, NextRacersList, NextRacePurse, NextRaceEntryFee, NextRaceName;
    int nextSurface, nextLength;
    bool hasAiEntered = false;
    public static int Purse = 500;
    public RaceData[] NextRaces;

    void Awake()
    {
        GuestManager.LoadGuestData();
        GuestManager.NextRacesTurtles.Clear();
    }

    void Start()
    {
        if (Purse > 3000) {
            Purse = 1000;
        }
        else
        {
            Purse += 500;
        }
        NextRacePurse.text = "Purse: " + Purse.ToString();
        int halfpurse = Purse / 10;
        NextRaceEntryFee.text = "Entry: " + halfpurse.ToString();
        nextLength = Random.Range(0, 1);
        nextSurface = Random.Range(0, 3);
        int randomRoll = Random.Range(1, 10);
        if (nextLength == 0)
        {
            NextRaceLengthHud.text = "Lenght: Short";
            if (nextSurface == 0)
            {
                NextRaceName.text = "Sprinters Stakes";
            }
            if (nextSurface == 1)
            {
                NextRaceName.text = "Sunset Sprint";
            }
            if (nextSurface == 2)
            {
                NextRaceName.text = "Sherwood Cup";
            }
        }
        if (nextLength == 1)
        {
            NextRaceLengthHud.text = "Length: Long";
            if (nextSurface == 0)
            {
                NextRaceName.text = "East Park";
            }
            if (nextSurface == 1)
            {
                NextRaceName.text = "Cairo Course";
            }
            if (nextSurface == 2)
            {
                NextRaceName.text = "Mazatlán Mile";
            }
        }
        if (nextSurface == 0)
        {
            NextRaceSurfaceHud.text = "Surface: Grass";
        }
        if (nextSurface == 1)
        {
            NextRaceSurfaceHud.text = "Surface: Sand";
        }
        if (nextSurface == 2)
        {
            NextRaceSurfaceHud.text = "Surface: Dirt";
        }

        IRC = this.GetComponent<TwitchIRC>();
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
        fakeMessage(Random.Range(1,3).ToString());


    }

    // Update is called once per frame
    void Update()
    {
        
        if (!hasAiEntered && Time.timeSinceLevelLoad > lengthOfGame-5)
        {
            for (int i = 0; i <= 10; i++)
            {
                fakeMessage(i.ToString());
            }
            hasAiEntered = true;
        }
        xposition = gameObject.transform.position.x;
        if (xposition > 10)
        {
            movedirection = "left";
        }
        if (xposition < -10)
        {
            movedirection = ("right");
        }
        if (movedirection == "right")
        {
            transform.Translate(Time.deltaTime * SpeedMultiplier, 0, 0);
        }
        if (movedirection == "left")
        {
            transform.Translate(-Time.deltaTime * SpeedMultiplier, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (IsMusicEnabled)
            {
                IsMusicEnabled = false;
                Debug.Log("Disabling Music");
            }
            else
            {
                IsMusicEnabled = true;
                Debug.Log("Enabling Music");
            }
        }
        if(Input.GetKeyDown(KeyCode.X)){
            lengthOfGame=3;
        }
        if (Time.timeSinceLevelLoad > lengthOfGame)
        {
            foreach (GuestData gD in GuestManager.AllGuests)
            {
                gD.bonusRoundBallsDropped = 0;
            }
            GuestManager.SaveGuestData();
            print("restarting");
            PlayerPrefs.SetInt("NextSurface", nextSurface);
            PlayerPrefs.SetInt("NextLength", nextLength);
            PlayerPrefs.SetInt("NextPurse", Purse);
            SceneManager.LoadScene("Race");
        }
        RaceStartingTimer.text = Mathf.RoundToInt(lengthOfGame - Time.timeSinceLevelLoad) + " Seconds";
    }

    void OnChatMsgRecieved(string msg)
    {

        TwitchIRC tIRC = GetComponent<TwitchIRC>();
        //parse from buffer.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        //print(msg);
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        GuestManager.CheckOrRegisterGuest(user);
        foreach (GuestData gD in GuestManager.AllGuests)
        {
            if (gD.guestName == user)
            {
                if (gD.bonusRoundBallsDropped < 6)
                {
                    gD.bonusRoundBallsDropped++;
                    DropBall(user);
                }
                if (msgString.CaseInsensitiveContains("stable"))
                {
                    StableReturner.StableRequest(gD, tIRC);
                }
                if (msgString.CaseInsensitiveContains("enter"))
                {
                    foreach(TurtleData tD in gD.ownedTurtles)
                    {
                        if (msgString.CaseInsensitiveContains(tD.name))
                        {
                            bool isTurtleAlreadyInRace = false;
                            foreach(TurtleData turtleInTheRaceSoFar in GuestManager.NextRacesTurtles)
                            {
                                if(turtleInTheRaceSoFar.name == tD.name) { 
                                    isTurtleAlreadyInRace = true;
                                    print("A turtle with that name is already entered");
                                }

                            }
                            print("Next races turtles count is at " + GuestManager.NextRacesTurtles.Count);
                            if (GuestManager.NextRacesTurtles.Count < 10)
                            {
                                if (isTurtleAlreadyInRace == false)
                                {
                                    if (gD.guestCash > (Purse / 10))
                                    {
                                        gD.guestCash -= Purse / 10;
                                        GuestManager.NextRacesTurtles.Add(tD);
                                        NextRacersList.text += GuestManager.NextRacesTurtles.Count + ") " + tD.name + " by " + tD.ownersName + "\n";
                                        Debug.Log(tD.name +  " entered into next race!");
                                    }
                                    else
                                    {
                                        Debug.Log("Someone tried to enter but they didnt have enough coins");
                                    }
                                }
                                else
                                {
                                    Debug.Log("Someone tried to enter but we think they were already in the list");
                                }
                            }
                            else
                            {
                                Debug.Log("Someone tried to enter a turtle but the race was full");
                            }
                        }
                    }
                }
            }
        }

        //BetManager.GetABet(msgString, user, this.gameObject);
        Debug.Log("<color=purple> msg = " + msg + "</color><color=blue> msgString = " + msgString + "</color><color=purple>user = " + user + "</color>");
        
        tIRC.SendCommand("PRIVMSG #" + tIRC.channelName + " : ");

    }




    public void DropBall(string droppersName)
    {
        GameObject newBall = Instantiate(ballToSpawn, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
        BallController ballsScriptRef = newBall.GetComponent<BallController>();
        ballsScriptRef.label.text = droppersName;

    }

    public void fakeMessage(string fM)
    {
        string fakeAIUsername = "turtlebot" + Random.Range(1,5);
        foreach(GuestData possibleGuest in GuestManager.AllGuests)
        {
            if(possibleGuest.guestName == fakeAIUsername)
            {
                if (possibleGuest.ownedTurtles.Count > 0)
                {
                    OnChatMsgRecieved(":" + fakeAIUsername + "!turtleracingalpha@turtleracingalpha.tmi.twitch.tv PRIVMSG #turtleracingalpha :Enter " + possibleGuest.ownedTurtles[Random.Range(0, possibleGuest.ownedTurtles.Count - 1)].name);
                }
            }
        }
        
    }
}
