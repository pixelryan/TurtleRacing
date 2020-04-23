using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class BallController : MonoBehaviour {

	// Use this for initialization
	public TextMeshPro label;
	bool didFireParticle;
	public GameObject coinsParticle;

	public Material [] PossibleMaterials;
		void Start () {
			
			MeshRenderer myMesh = GetComponent<MeshRenderer>();
			myMesh.material = PossibleMaterials[Random.Range(0,PossibleMaterials.Length)];
		
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y <-20){
                 Destroy(gameObject);
		}
		label.transform.rotation = Camera.main.transform.rotation;
	}

	void OnCollisionEnter (Collision col){
		if(col.gameObject.tag == "Finish")
        {
			if(!didFireParticle){
			Animator colAnim = col.transform.parent.parent.GetComponent<Animator>();
			colAnim.SetTrigger("Open");
			GameObject coins_Particle = Instantiate(coinsParticle, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.Euler(new Vector3(-90,0,0)));
			didFireParticle = true;
			int rewardToGive = 5;
			rewardToGive = int.Parse(col.transform.parent.parent.name);
			TextMeshPro coinText = coins_Particle.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
			coinText.text = "+" + rewardToGive;
			foreach(GuestData gD in GuestManager.AllGuests){
				if(gD.guestName == label.text){
					Debug.Log(gD.guestName + "Hit the jackpot!");
					gD.guestCash+=rewardToGive;
					if(rewardToGive == 100){
						GameObject bottomToaster = GameObject.Find("Toaster");
                		ToasterManager toastScriptRef = bottomToaster.GetComponent<ToasterManager>();
						toastScriptRef.ShowAToaster(gD.guestName, "Hit the JACKPOT!");
					}
				}
			}
			}
		}
	}
}
