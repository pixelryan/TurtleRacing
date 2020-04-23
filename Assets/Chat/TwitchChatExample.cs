using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchChatExample : MonoBehaviour
{
    public int maxMessages = 100; //we start deleting UI elements when the count is larger than this var.
    private LinkedList<GameObject> messages =
        new LinkedList<GameObject>();
    public UnityEngine.UI.InputField inputField;
    public UnityEngine.UI.Button submitButton;
    public UnityEngine.RectTransform chatBox;
    public UnityEngine.UI.ScrollRect scrollRect;
    private TwitchIRC IRC;
    //when message is recieved from IRC-server or our own message.
    void OnChatMsgRecieved(string msg)
    {
        //parse from buffer.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);
        
        GuestManager.CheckOrRegisterGuest(user);
        
        BetManager.GetABet(msgString, user, this.gameObject);
        //Debug.Log("<color=purple> msg = " + msg + "</color><color=blue> msgString = " + msgString + "</color><color=purple>user = " + user + "</color>");
       

		if (msgString == "Enter") {

			Debug.Log ("Spawn a spectator!"); //Possible future feature to spawn audience avatar
            foreach(GuestData possibleGuest in GuestManager.AllGuests)
            {
                if(possibleGuest.guestName == user)
                {
                    if (possibleGuest.ownedTurtles.Count > 0) {
                        string numbersInMessage = Regex.Match(msgString, @"\d+").Value;
                        int numbersInMessageAsInt = int.Parse(numbersInMessage);
                        //BonusRoundManager.TurtlesToEnterNextRace.SetValue(possibleGuest.ownedTurtles[numbersInMessageAsInt-1], BonusRoundManager.TurtlesToEnterNextRace.Length); //doing this elsewhere now
                    }
                }
            }
		}
        if (msgString.CaseInsensitiveContains("balance"))
        {
            //send a PM with their balance
        }

        //remove old messages for performance reasons.
        if (messages.Count > maxMessages)
        {
            Destroy(messages.First.Value);
            messages.RemoveFirst();
        }

        //add new message.
        //CreateUIMessage(user, msgString);
    }
    void CreateUIMessage(string userName, string msgString)
    {
        Color32 c = ColorFromUsername(userName);
        string nameColor = "#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");
        GameObject go = new GameObject("twitchMsg");
        var text = go.AddComponent<UnityEngine.UI.Text>();
        var layout = go.AddComponent<UnityEngine.UI.LayoutElement>();
        go.transform.SetParent(chatBox);
        messages.AddLast(go);

        layout.minHeight = 20f;
        text.text = "<color=" + nameColor + "><b>" + userName + "</b></color>" + ": " + msgString;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        scrollRect.velocity = new Vector2(0, 1000f);
    }
    //when Submit button is clicked or ENTER is pressed.
    public void OnSubmit()
    {
        if (inputField.text.Length > 0)
        {
            IRC.SendMsg(inputField.text); //send message.
            CreateUIMessage(IRC.nickName, inputField.text); //create ui element.
            inputField.text = "";
        }
    }
    Color ColorFromUsername(string username)
    {
		if (username == "") {
			return Color.red;
		}
        //Random.seed = username.Length + (int)username[0] + (int)username[username.Length - 1];
        return new Color(Random.Range(0.25f, 0.55f), Random.Range(0.20f, 0.55f), Random.Range(0.25f, 0.55f));
    }
    // Use this for initialization
    void Start()
    {
        IRC = this.GetComponent<TwitchIRC>();
        IRC.SendCommand("CAP REQ :twitch.tv/commands");
        //IRC.SendCommand("CAP REQ :twitch.tv/tags"); //register for additional data such as emote-ids, name color etc.
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
    }


    public void fakeMessage(string fM){  //made for testing odds
            int randomTurtleBot = Random.Range(1,5);
            if(fM.Contains("bid")){
                OnChatMsgRecieved(":turtlebot" + randomTurtleBot + "!turtleracingalpha@turtleracingalpha.tmi.twitch.tv PRIVMSG #turtleracingalpha : " +fM);
                return;
            }
            int randomBetType = Random.Range(1,4);
            string betTypeToAppend ="";
            if(randomBetType==1){
                betTypeToAppend="win";
            }
            if(randomBetType==2){
                betTypeToAppend="place";
            }
            if(randomBetType==3){
                betTypeToAppend="show";
            }
             
           OnChatMsgRecieved(":turtlebot" + randomTurtleBot + "!turtleracingalpha@turtleracingalpha.tmi.twitch.tv PRIVMSG #turtleracingalpha :Bet " +fM +" 5 " +betTypeToAppend);
    }
}
