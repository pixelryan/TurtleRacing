using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

	// Use this for initialization
	public GameObject [] Panels;
	public RaceManager RCManScriptRef;
	private float panelTimer;

	private int currentPanel = 1;
	public float panelDelay = 3;
	void Start () {
		Panels[0].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		if(RCManScriptRef.hasRaceStarted){
			gameObject.SetActive(false);
		}
		if(Time.time > panelDelay){
			if(currentPanel==3){
				gameObject.SetActive(false);
				return;
			}
			Panels[currentPanel-1].SetActive(false);
			Panels[currentPanel].SetActive(true);
			panelDelay = Time.time + panelDelay;
			currentPanel++;
		}
		
	}
}
