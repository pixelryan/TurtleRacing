using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleNamer : MonoBehaviour {

	// Use this for initialization
	string UsedNames = "";
	public string [] PossibleNames;
	public static int namesGiven;
	public static string NameToGive;

	public string GiveNewRandomName(){
		string newNameToGive = PossibleNames[Random.Range(0,PossibleNames.Length)];
		if(UsedNames.Contains(newNameToGive)){
			newNameToGive = PossibleNames[Random.Range(0,PossibleNames.Length)];
			if(UsedNames.Contains(newNameToGive)){
				newNameToGive = PossibleNames[Random.Range(0,PossibleNames.Length)];
				if(UsedNames.Contains(newNameToGive)){
					newNameToGive = PossibleNames[Random.Range(0,PossibleNames.Length)];
					if(UsedNames.Contains(newNameToGive)){
						newNameToGive = PossibleNames[Random.Range(0,PossibleNames.Length)];
					}
				}
			}
		}
		UsedNames = UsedNames+newNameToGive;
		namesGiven++;
		return(newNameToGive);
	}
}
