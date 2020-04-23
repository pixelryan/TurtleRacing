using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;
using System.Linq;

public class TopPlayersPanel : MonoBehaviour {

	public TextMeshProUGUI PlayersList;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		GuestManager.AllGuests = GuestManager.AllGuests.OrderByDescending(w => w.guestCash).ToList(); //Sorts guests by money
		string newPlayersListText = "";
		int playersInTop25 = 0;
		if(GuestManager.AllGuests == null){
			print("player list is still blank");
			return;
		}
		if(GuestManager.AllGuests.Count >0){
			foreach(GuestData gD in GuestManager.AllGuests){
				if(playersInTop25<20){
					playersInTop25++;
					string thisPlayersText = playersInTop25 + ")"+ gD.guestName + "..." + gD.guestCash.ToString("n0") + "\n";
					newPlayersListText = newPlayersListText + thisPlayersText;
				}
			}
			PlayersList.text = newPlayersListText;
		}
		else{
			/*GuestData tempGuy = new GuestData();
			tempGuy.guestName = "TurtleBot1";
			tempGuy.guestCash = 200;
			GuestManager.AllGuests.Add(tempGuy);*/

		}
	}
}
