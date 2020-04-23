using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ticker : MonoBehaviour
{

    public TextMeshProUGUI scrollingText;
    public float scrollSpeed;


    // Update is called once per frame
    void Update()
    {
        scrollingText.transform.position -= new Vector3(scrollSpeed * Time.deltaTime, 0, 0);

        scrollingText.text = "";
        if (GuestManager.AllGuests.Count > 0)
        {
            foreach (GuestData gD in GuestManager.AllGuests)
            {

                string thisPlayersText = gD.guestName + " " + gD.guestCash.ToString("n0") + "  /  ";
                scrollingText.text = scrollingText.text + thisPlayersText;
            }
        }
		//print(scrollingText.renderedWidth + " "+scrollingText.transform.position.x);

		if(scrollingText.transform.position.x < -scrollingText.renderedWidth + (Screen.width/2) ){
				scrollingText.transform.position = new Vector3(Screen.width + (Screen.width/2) , scrollingText.transform.position.y, scrollingText.transform.position.z);
		}
    }
}
