using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TurtleDetailsGridFiller : MonoBehaviour {

    public TMP_Text[] details;
    public int TurtleNumber;
    public RaceManager raceManagerRef;
	// Use this for initialization
	void Start () {
        details[0].text = raceManagerRef.TurtlesInTheRace[TurtleNumber-1].name;
        details[1].text = "A: " + raceManagerRef.TurtlesInTheRace[TurtleNumber - 1].GetComponent<TurtleAI>().myAcceleration ;
        details[2].text = "E: " + raceManagerRef.TurtlesInTheRace[TurtleNumber - 1].GetComponent<TurtleAI>().myAcceleration;
        details[3].text = "F: ?";



    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
