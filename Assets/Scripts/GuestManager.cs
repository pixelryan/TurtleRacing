// ▄▀▀▀▀▄   ▄▀▀▄ ▄▀▀▄  ▄▀▀█▄▄▄▄  ▄▀▀▀▀▄  ▄▀▀▀█▀▀▄     ▄▀▀▄ ▄▀▄  ▄▀▀█▄   ▄▀▀▄ ▀▄  ▄▀▀█▄   ▄▀▀▀▀▄   ▄▀▀█▄▄▄▄  ▄▀▀▄▀▀▀▄ 
//█        █   █    █ ▐  ▄▀   ▐ █ █   ▐ █    █  ▐    █  █ ▀  █ ▐ ▄▀ ▀▄ █  █ █ █ ▐ ▄▀ ▀▄ █        ▐  ▄▀   ▐ █   █   █ 
//█    ▀▄▄ ▐  █    █    █▄▄▄▄▄     ▀▄   ▐   █        ▐  █    █   █▄▄▄█ ▐  █  ▀█   █▄▄▄█ █    ▀▄▄   █▄▄▄▄▄  ▐  █▀▀█▀  
//█     █ █  █    █     █    ▌  ▀▄   █     █           █    █   ▄▀   █   █   █   ▄▀   █ █     █ █  █    ▌   ▄▀    █  
//▐▀▄▄▄▄▀ ▐   ▀▄▄▄▄▀   ▄▀▄▄▄▄    █▀▀▀    ▄▀          ▄▀   ▄▀   █   ▄▀  ▄▀   █   █   ▄▀  ▐▀▄▄▄▄▀ ▐ ▄▀▄▄▄▄   █     █   
//▐                    █    ▐    ▐      █            █    █    ▐   ▐   █    ▐   ▐   ▐   ▐         █    ▐   ▐     ▐   
//                     ▐                ▐            ▐    ▐            ▐                          ▐                  
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;



public static class GuestManager
{

    private static string guestBookDataFileName = "data.txt";
    //private string guestDataFileName = "data.json";
    public static string FullGuestBook;
    //public static List<BetData> CurrentRaceBets = new List<BetData>();
    public static List<RaceResultData> CurrentRaceresults = new List<RaceResultData>();
    public static List<GuestData> AllGuests = new List<GuestData>();
    public static List<TurtleData> NextRacesTurtles = new List<TurtleData>();
    public static string TwitchNameText, OauthText;
    static GameObject bottomToaster;

    public static int DailyReward = 200;

    public static bool CaseInsensitiveContains(this string text, string value,
        StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
    {
        return text.IndexOf(value, stringComparison) >= 0;
    }

    public static void CheckOrRegisterGuest(string chattersName)
    {
        //TimeSpan timeSinceLastLoggedIn = currentDate.Subtract()
        //Debug.Log("checking if " + chattersName + " user exists");
		bool wasAlreadyRegistered = false;
        foreach (GuestData gD in AllGuests)
        {
        
            if (gD.guestName == chattersName)
            {
                wasAlreadyRegistered = true;
                DateTime currentDate = System.DateTime.Now.Date;
                if (gD.lastLoginBonusDate != currentDate.ToBinary().ToString())
                {
                    //Debug.Log("They are back! Give <color=red>" + chattersName + " </color> some money. Last we saw them was " + gD.lastLoginBonusDate + " and now its " + currentDate.ToBinary().ToString());
                    gD.guestCash += DailyReward;
                    gD.lastLoginBonusDate = System.DateTime.Now.Date.ToBinary().ToString();
                    bottomToaster = GameObject.Find("Toaster");
                    ToasterManager toastScriptRef = bottomToaster.GetComponent<ToasterManager>();
                    toastScriptRef.ShowAToaster("Welcome " + gD.guestName, "+ " + DailyReward.ToString() + " Daily Login Bonus");
                }
            }
        }
		 if (!wasAlreadyRegistered)
            {
                //FullGuestBook = FullGuestBook + "Guest:" + chattersName;
                GuestData newGuestToRegister = new GuestData();
                newGuestToRegister.guestName = chattersName;
                newGuestToRegister.guestCash = DailyReward;
                Debug.Log("Welcome <color=red>" + chattersName + " </color>. Starting money awarded");
                string currentTimeStamp = System.DateTime.Now.ToBinary().ToString();
                string currentDay = System.DateTime.Now.Date.ToBinary().ToString();
                newGuestToRegister.registeredDate = currentTimeStamp;
                newGuestToRegister.lastLoginBonusDate = currentDay;
                AllGuests.Add(newGuestToRegister);
                bottomToaster = GameObject.Find("Toaster");
                ToasterManager toastScriptRef = bottomToaster.GetComponent<ToasterManager>();
                toastScriptRef.ShowAToaster("Welcome " + newGuestToRegister.guestName, "+ " + DailyReward.ToString() + " Daily Login Bonus");
            }
    }

    public static void SaveGuestData()
    {
        //create path to look for data to load
        string filePath = Path.Combine(Application.streamingAssetsPath, guestBookDataFileName);
        if (File.Exists(filePath))
        {
            //read all the text into a string
            var GuestDataSavedAsJson = Newtonsoft.Json.JsonConvert.SerializeObject(AllGuests, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, GuestDataSavedAsJson);
            Debug.Log("Guest Data File Saved!");
        }
        else
        {
            //File.Create(filePath);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(AllGuests, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
            Debug.Log("Guest Data File Created and Saved!");
            //string guestDataAsJson = JsonUtility.ToJson(GuestManager.AllGuests);
            //BinaryFormatter bf = new BinaryFormatter ();
            //FileStream file = File.Open (Application.persistentDataPath + "/" + "guestsaves.dat", FileMode.Open);
            //GuestData data = (GuestData)bf.Deserialize(file);
        }
    }
    public static void LoadGuestData()
    {
        //create path to look for data to load
        string filePath = Path.Combine(Application.streamingAssetsPath, guestBookDataFileName);
        if (File.Exists(filePath))
        {
            //read all the text into a string

            var json = File.ReadAllText(filePath);
            GuestManager.AllGuests = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GuestData>>(json);
            Debug.Log("Guest Data Loaded! In the pipe, 5x5!");
            //string guestDataAsJson = File.ReadAllText(filePath);
            //GuestManager.AllGuests = JsonUtility.FromJson<GuestData>(guestDataAsJson);
            //FullGuestBook = File.ReadAllText(filePath);
            //string dataAsJson = File.ReadAllText(filePath);
            //GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            Debug.Log("Tried to load and no save file existed");
        }
        string twitchSettingsPath = Path.Combine(Application.streamingAssetsPath, "TwitchName.txt");
        if(File.Exists(twitchSettingsPath)){
            TwitchNameText = File.ReadAllText(twitchSettingsPath);
        }
        string oauthSettingsPath = Path.Combine(Application.streamingAssetsPath, "Oauth.txt");
        if(File.Exists(oauthSettingsPath)){
            OauthText = File.ReadAllText(oauthSettingsPath);
        }
    }

    public static void ClearNextRaceTurtles()
    {
        NextRacesTurtles = new List<TurtleData>();
    }

}


