using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurtleForSale : MonoBehaviour
{

    // Use this for initialization

    public TextMeshProUGUI titleUI, accellUI, endUI, favSurfUI, bidUI, topBidderUI, biddingInstructions;
    public GameObject RaceManagerGameRef;
    public string topBiddersName, myName;
    public int currentBid;
    Animator myAnimator;
    float idleCounter = 0;
   

    int rA, rB, rC;
    void Start() { 
        myName = RaceManagerGameRef.gameObject.GetComponent<TurtleNamer>().GiveNewRandomName();
        titleUI.text = myName + " for Auction";
        biddingInstructions.text = "Type !Bid " + myName + " 10 to bid on me!";
        float statsTotal = Random.Range(4,7);
        float fN = statsTotal - (statsTotal/2.5f);
        statsTotal -= fN;
        rA = Mathf.RoundToInt(statsTotal);
        rB = Mathf.RoundToInt(fN);
        rC = Random.Range(1, 4);
        accellUI.text = "Acceleration: " + rA;
        endUI.text = "Endurance: " + rB;
        string favSurfaceText = " ";
        if (rC == 1)
        {
            favSurfaceText = "Grass";
        }
		if (rC == 2)
        {
            favSurfaceText = "Sand";
        }
		if (rC == 3)
        {
            favSurfaceText = "Dirt";
        }
        favSurfUI.text = "Favorite Surface: " + favSurfaceText;
        TurtleData newTurtleToRegister = new TurtleData();
        newTurtleToRegister.favoriteSurface = favSurfaceText;
        newTurtleToRegister.baseAcceleration = rA;
        newTurtleToRegister.baseEndurance = rB;
        newTurtleToRegister.name = myName;
        newTurtleToRegister.MyScaleX = Random.Range(-10, 10);
        newTurtleToRegister.MyScaleY = Random.Range(-10, 10);
        TurtleAuctionManager.TurtlesForAuction.Add(newTurtleToRegister);
        myAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (idleCounter < Time.time)
        {
            myAnimator.SetTrigger("Play Idle");
            idleCounter = Time.time + Random.Range(1, 2);
        }
            bidUI.text = "Current Bid: " + currentBid;
        topBidderUI.text = "Top Bidder: " + topBiddersName;
    }

    public void GetABid(int bid, string biddersName){
        if(bid>currentBid){
            foreach(GuestData possibleGuest in GuestManager.AllGuests)
            {
                if(possibleGuest.guestName == biddersName)
                {
                    if(bid <= possibleGuest.guestCash)
                    {
                        possibleGuest.guestCash -= bid;
                        foreach (GuestData possiblePreviousTopBidder in GuestManager.AllGuests)
                        {
                            if(possiblePreviousTopBidder.guestName == topBiddersName)
                            {

                                possiblePreviousTopBidder.guestCash += currentBid;
                            }
                        }
                        currentBid = bid;
                        topBiddersName = biddersName;
                    }
                }
            }
       
            
            //When the auction is over, save the top bidders name as the turtles owners name.
            foreach(TurtleData tD in TurtleAuctionManager.TurtlesForAuction)
            {
                if(myName == tD.name)
                {
                    tD.soldFor = bid;
                    tD.ownersName = biddersName;
                }
            }
        }
    }

    public void CollectTurtleInfo()
    {

    }
}
