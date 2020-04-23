using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RandomPathPicker : MonoBehaviour {

    public CinemachinePathBase [] possiblePaths;
	// Use this for initialization
	void Start () {
        int randomPathNumber = Random.Range(0, possiblePaths.Length);
        CinemachineDollyCart cartScriptRef = GetComponent<CinemachineDollyCart>();
        cartScriptRef.m_Path = possiblePaths[randomPathNumber];
        if (randomPathNumber ==1)
        {
            cartScriptRef.m_Speed = 1f;
        }
        if (randomPathNumber == 2)
        {
            cartScriptRef.m_Speed = 0.9f;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
