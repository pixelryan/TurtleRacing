using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;


public class TurtleAI : MonoBehaviour {

    public Color myColor;
	public float BaseSpeed, BaseSpeedAnimMultiplier, SpeedChangeTickRate;
	bool hasRaceStarted, hasPickedMaterial, didIFinish;
	private Animator myAnimator;
	private float SpeedChangeTimer, secondWindTimer, idleCounter;
    public static string usedMats = "", RaceResultsColumn1 = "", RaceResultsColumn2 = "", RaceResultsColumn3 = "", RaceResultsColumn4 = "", mySurface;
	public static int HowManyTurtlesFinished;
	public float myOdds = 23, howMuchIsBetOnMe = 0, howMuchIsBetOnMeToShow = 0, howMuchIsBetOnMeToPlace = 0;
	public float myRealOdds, percentFinished;
	public CinemachineVirtualCamera ShoulderCam;
	public GameObject RaceManagerReference, UIName, UIOdds, UIOddsTitle, OverHeadName;
	public Material [] PossibleMaterials;
	public SkinnedMeshRenderer theMesh;
	private TextMeshProUGUI oddsGui, inRaceResultsText;
	RaceManager RaceManagerScriptRef;
    public int myEndurance, myAcceleration;
    public TurtleData myTurtleData;
    public AudioClip[] footsteps;
    public int myNumber;
    
    void Start () {
        if (myTurtleData.baseAcceleration > 0)
        {
            gameObject.name = myTurtleData.name;
            myEndurance = myTurtleData.baseEndurance;
            myAcceleration = myTurtleData.baseAcceleration;
            mySurface = myTurtleData.favoriteSurface;
        }
        else
        {
            myAcceleration = Random.Range(1, 4);
            myEndurance = Random.Range(1, 4);
        }
        transform.localScale = new Vector3(1 + (myTurtleData.MyScaleX * 0.01f), 1 + (myTurtleData.MyScaleY * 0.01f), 1);
        RaceResultsColumn1 = "";
		RaceResultsColumn2 = "";
		RaceResultsColumn3 = "";
        RaceResultsColumn4 = "";
		idleCounter = Random.Range(0,15);
		usedMats="";
		
		HowManyTurtlesFinished = 0;
		RaceManagerScriptRef =  RaceManagerReference.GetComponent<RaceManager>();
        inRaceResultsText = RaceManagerScriptRef.inRaceResultsGiuText;
		myAnimator = gameObject.GetComponent<Animator> ();
		BaseSpeed += Random.Range (-10,10)*.01f;
		SpeedChangeTimer = SpeedChangeTickRate;
		oddsGui = UIOdds.GetComponent<TextMeshProUGUI>();
		//TurtleNamer TurlteNamerScriptRef = RaceManagerReference.GetComponent<TurtleNamer>();
		//gameObject.name = TurlteNamerScriptRef.GiveNewRandomName();
		//gameObject.name = TurlteNamerScriptRef.PossibleNames[Random.Range(0,TurlteNamerScriptRef.PossibleNames.Length -1)];
		UIName.GetComponent<TextMeshProUGUI>().text = gameObject.name;
	}
	
	// Update is called once per frame
	void Update () {
		percentFinished = transform.position.z / RaceManagerScriptRef.FinishLineLocator.transform.position.z;
        float distanceTraveled = RaceManagerScriptRef.StartingLineLocator.transform.position.z + transform.position.z;
		if(!hasPickedMaterial){ //To pick a random color
            PickARandomMaterial();

        }

		if(!RaceManagerScriptRef.hasRaceStarted){
			if(idleCounter < Time.time){
				myAnimator.SetTrigger("Play Idle");
				idleCounter = Time.time + Random.Range(10,20);
			}

			//calculate odds the old way, based on bets
			if(howMuchIsBetOnMe > 0){
				myRealOdds = Mathf.RoundToInt((OddsDisplay.CurrentTotalPot / howMuchIsBetOnMe));
			}
			else{
				myRealOdds = OddsDisplay.CurrentTotalPot;
			}
            
			oddsGui.text =myRealOdds.ToString()+"/1";

            
		}

		if (RaceManagerScriptRef.hasRaceStarted) {
			myAnimator.SetTrigger ("Start Walking");
			myAnimator.SetFloat ("MoveAnimSpeed", BaseSpeed / BaseSpeedAnimMultiplier);
			transform.Translate (Vector3.forward * BaseSpeed * Time.deltaTime);
		}
        if (Time.time >= SpeedChangeTimer && RaceManagerScriptRef.hasRaceStarted){
            if (percentFinished < 0.8f){
                SpeedChangeTimer = Time.time + SpeedChangeTickRate;
                BaseSpeed += Random.Range(-50, 50) * .001f;
                if(percentFinished < 0.1f * myAcceleration)
                {
                    BaseSpeed += Random.Range(10, 50) * .001f;
                    if(RaceManagerScriptRef.TrackName == mySurface)
                    {
                        BaseSpeed += 0.005f;
                    }
                }
                if(distanceTraveled > 40 + (myEndurance * 10) && BaseSpeed >0.8f)
                {
                    BaseSpeed -= Random.Range(10, 50) * .001f;
                }

                if (BaseSpeed <= 0.83f)
                {
                    BaseSpeed = 1f;
                }


            }
			if(percentFinished>1){ //we passed the finish line
				if(BaseSpeed > 0.01f){
					BaseSpeed -= 0.002f;
				}

			}

			//print ("Changed speeds" + Time.time + "   " +SpeedChangeTimer + "   " + SpeedChangeTickRate);
		}
		//float dist = Vector3.Distance (FinishLineLocator.transform.position, transform.position);
		//print(gameObject.name + dist);
		if(gameObject.transform.position.z>RaceManagerScriptRef.FinishLineLocator.transform.position.z && !didIFinish){
			//record race result.
			RaceResultData aSingleResult = new RaceResultData();
            aSingleResult.OwnerName = myTurtleData.ownersName;
			aSingleResult.TurtleName = gameObject.transform.name;
			aSingleResult.FinishingPlace = HowManyTurtlesFinished + 1;
            float PurseDistribution = PayOutPossibleOwner();
            string statBoostString = "";
            if (aSingleResult.FinishingPlace < 6)
            {
                
                int amountOfStatToBoost = Random.Range(1,3);
                int randomChanceOfBigBoost = Random.Range(1, 20);
                if(randomChanceOfBigBoost == 9)
                {
                    amountOfStatToBoost = amountOfStatToBoost * 3; //random tripple boost
                }
                statBoostString = "+" + amountOfStatToBoost;
                int randomStat = Random.Range(0, 2);
                if(randomStat == 0)
                {
                    if (!myTurtleData.ownersName.CaseInsensitiveContains("turtlebot"))
                    {
                        myTurtleData.baseAcceleration += amountOfStatToBoost;
                    }
                    statBoostString += " Acceleration";
                }
                if(randomStat == 1)
                {
                    if (!myTurtleData.ownersName.CaseInsensitiveContains("turtlebot"))
                    {
                        myTurtleData.baseEndurance += amountOfStatToBoost;
                    }
                    statBoostString += " Endurance";
                }
            }
            //aSingleResult.StatBoost = 
            GuestManager.CurrentRaceresults.Add(aSingleResult);
			RaceResultsColumn1 += (HowManyTurtlesFinished +1) +")<nobr>"+gameObject.name + " \n";
            inRaceResultsText.text = RaceResultsColumn1;
			RaceResultsColumn2 += myRealOdds.ToString() +"/1 \n";
			RaceResultsColumn3 += howMuchIsBetOnMe.ToString()+" \n";
            RaceResultsColumn4 += PurseDistribution + " " + statBoostString + " \n";
			//RaceResults += "</nobr>" + (HowManyTurtlesFinished+1) + ". " + gameObject.name + " - Total Pool: " + howMuchIsBetOnMe + " Odds: " +myRealOdds +"</nobr> \n ";
			HowManyTurtlesFinished++;
			//print(RaceResults);
			didIFinish = true;
		}


		
	}

    void PickARandomMaterial()
    {
        
        Material matToTryThisTime = PossibleMaterials[Random.Range(0, PossibleMaterials.Length)];
        
        //theMesh.material = matToTryThisTime;
        /*if (usedMats.Contains(matToTryThisTime.name))
        {
            return;
        }
        
        usedMats = usedMats + matToTryThisTime.name;
        */
        hasPickedMaterial = true;
        //UIName.GetComponent<TextMeshProUGUI>().color = myColor;
        //oddsGui.color = myColor;
        //oddsGui.text = oddsReturner(myOdds);
        //UIOddsTitle.GetComponent<TextMeshProUGUI>().color = myColor;
        OverHeadName.GetComponent<TextMeshProUGUI>().color = myColor;
        OverHeadName.GetComponent<TextMeshProUGUI>().text = gameObject.name;
    }

    float PayOutPossibleOwner()
    {
        float reward = 0;
        if (HowManyTurtlesFinished + 1 == 1)
        {
            reward = RaceManagerScriptRef.Purse * 0.6f;
        }
        if (HowManyTurtlesFinished + 1 == 2)
        {
            reward = RaceManagerScriptRef.Purse * 0.2f;
        }
        if (HowManyTurtlesFinished + 1 == 3)
        {
            reward = RaceManagerScriptRef.Purse * 0.11f;
        }
        if (HowManyTurtlesFinished + 1 == 4)
        {
            reward = RaceManagerScriptRef.Purse * 0.06f;
        }
        if (HowManyTurtlesFinished + 1 == 5)
        {
            reward = RaceManagerScriptRef.Purse * 0.03f;
        }
        foreach (GuestData possibleOwner in GuestManager.AllGuests)
        {
            if (myTurtleData.ownersName == possibleOwner.guestName)
            {
                possibleOwner.guestCash += reward;
                //print(reward + " given to " + myTurtleData.ownersName + " for their turtle finishing ");   
            }
        }
        return (Mathf.Round(reward));
    }

    private void step()
    {
        AudioSource.PlayClipAtPoint(footsteps[Random.Range(0, footsteps.Length)], transform.position);
    }
}
