using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundClick : MonoBehaviour
{
	GameObject player;
	PlayerScript playerScript;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerScript> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnMouseUpAsButton()
	{
		// where user click is not the same as the player position
		if (player.transform.position != gameObject.transform.position)
		{
			playerScript.FinalTarget = gameObject.transform.position;
		}
	}
}
