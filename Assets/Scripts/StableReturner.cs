using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableReturner : MonoBehaviour {

	public static void StableRequest (GuestData stableRequester, TwitchIRC requestorIRC)
    {
        string stableString = "Stable: ";
        int ownedturtlecount = 0;
        foreach (TurtleData tD in stableRequester.ownedTurtles)
        {
            ownedturtlecount++;
            stableString = ownedturtlecount + ") " + tD.name + "  Acceleration: " + tD.baseAcceleration + " Endurance: " + tD.baseEndurance + " Favorite Surface: " + tD.favoriteSurface;
            requestorIRC.SendCommand("PRIVMSG #" + requestorIRC.channelName + " :/w " + stableRequester.guestName + " " + stableString);
        }
        if (stableRequester.ownedTurtles.Count == 0) {
            requestorIRC.SendCommand("PRIVMSG #" + requestorIRC.channelName + " :/w " + stableRequester.guestName + " You don't own any turtles. You can bid for one next race during the auction.");
        }
        requestorIRC.SendCommand(stableString);
    }
}
