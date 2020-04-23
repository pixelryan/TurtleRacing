using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auctioneer : MonoBehaviour {

    public static void FinalizeAuction(GameObject raceManagerGameObjectRef)
    {
        TurtleData soldTurtle = TurtleAuctionManager.TurtlesForAuction[0];      
        Debug.Log(soldTurtle.name + "Auction over " + soldTurtle.ownersName);
        foreach (GuestData pB in GuestManager.AllGuests)
        {
            if (pB.guestName == soldTurtle.ownersName)
            {
                pB.ownedTurtles.Add(soldTurtle);
            }
        }
    }
}
