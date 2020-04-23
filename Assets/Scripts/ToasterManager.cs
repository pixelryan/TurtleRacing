using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class ToasterManager : MonoBehaviour {

	
	public TextMeshProUGUI Header, Body;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowAToaster(string headerText, string bodyText){
		Header.text = headerText;
		Body.text = bodyText;
		PlayableDirector pD = gameObject.GetComponent<PlayableDirector>();
		pD.Pause();
		pD.Play();
	}

}
