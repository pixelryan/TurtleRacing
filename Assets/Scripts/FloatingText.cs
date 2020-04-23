using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour {

	public TextMeshPro tm;
    float starttime, fademultiplier;
	// Use this for initialization
	void Start () {
         tm = GetComponent<TextMeshPro>();
        starttime = Time.time;
        if(tm.text.Contains("100")){ starttime += 1.5f; } 
	}
	
	// Update is called once per frame
	void Update () {
        if (starttime + 0.5f < Time.time)
        {
            if (tm.color.a < 0.5f)
            {
                tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a * 0.95f);
            }
            else
            {
                tm.color = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a * 0.7f);
            }
        }
        transform.Translate(Vector3.up * Time.deltaTime * 1.15f);
        
	}
}
