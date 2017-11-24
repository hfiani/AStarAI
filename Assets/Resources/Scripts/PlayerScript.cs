using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
	public Vector3 FinalTarget = Vector3.one * 99999;

	[SerializeField] private float speed = 0.5f;

	Vector3 _position;
	Animator anim;
	Stack roadToTake = new Stack();
	bool startMoving = false;
	Vector3 nextTarget = Vector3.one * 99999;
	float t;
	Canvas CanvasObj;
	Canvas Win;
	Text Score;
	int pokeballs = 0;

	// Use this for initialization
	void Start ()
	{
		CanvasObj = GameObject.Find ("GameEnded").GetComponent<Canvas>();
		Win = GameObject.Find("Win").GetComponent<Canvas>();
		Score = GameObject.FindGameObjectWithTag ("score").GetComponent<Text>();
		Score.text = "x0 of " + MapCreate.Pokeballs;

		_position = transform.position - MapCreate.PositionStarter;
		anim = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update ()
	{
		// not moving and there is a target and we did not reach it
		// => get A* path and start moving
		if (!startMoving && FinalTarget != Vector3.one * 99999 && transform.position != FinalTarget)
		{
			roadToTake = AStarFunctions.AStar (_position, FinalTarget - MapCreate.PositionStarter);

			FinalTarget = Vector3.one * 99999;
			startMoving = true;
			nextDirection ();
			t = 0;
			anim.SetBool ("walk", true);
		}
	}

	void FixedUpdate()
	{
		// we have a path and game not ended
		if (startMoving && !MapCreate.GameEnded)
		{
			int typeTile = MapCreate.Map [-(int)_position.y, (int)_position.x];
			t += Time.deltaTime * speed / MapCreate.TileCost[typeTile];
			transform.position = Vector3.Lerp (_position + MapCreate.PositionStarter, nextTarget, t);
			// we reached the next block
			if (Vector3.Distance(transform.position, nextTarget) < 0.001f)
			{
				t = 0;
				_position = nextTarget - MapCreate.PositionStarter;
				// no more blocks
				// => we reached target
				if (roadToTake.Count == 0)
				{
					startMoving = false;
					nextTarget = Vector3.zero;
					FinalTarget = Vector3.one * 99999;
					anim.SetBool ("walk", false);
				}
				// else
				// => get next direction
				else
				{
					nextDirection ();
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		// player collided with a pokeball
		// => get it, destroy the object, increment score
		if (c.tag.Equals ("pokeball"))
		{
			pokeballs++;
			Score.text = "x" + pokeballs + " of " + MapCreate.Pokeballs;
			Destroy (c.gameObject);
			// number of pokeballs to collect reached
			// => victory
			if (pokeballs == MapCreate.Pokeballs)
			{
				anim.SetBool ("walk", false);
				roadToTake.Clear ();
				CanvasObj.enabled = true;
				Win.enabled = true;
				GameObject.Find ("Score").GetComponent<Canvas> ().enabled = false;
				MapCreate.GameEnded = true;
			}
		}
	}

	/// <summary>
	/// Gets the next direction and changes animation accordingly
	/// </summary>
	void nextDirection()
	{
		Vector2 nextPosition = ((Vector2)roadToTake.Pop ());
		nextTarget = new Vector3 (nextPosition.y, -nextPosition.x, transform.position.z) + MapCreate.PositionStarter;
		if (_position.x + MapCreate.PositionStarter.x - nextTarget.x > 0) // left
		{
			anim.SetFloat ("direction", 0.33f);
		}
		else if (_position.x + MapCreate.PositionStarter.x - nextTarget.x < 0) // right
		{
			anim.SetFloat ("direction", 1.00f);
		}
		else if (_position.y + MapCreate.PositionStarter.y - nextTarget.y > 0) // down
		{
			anim.SetFloat ("direction", 0.00f);
		}
		else if (_position.y + MapCreate.PositionStarter.y - nextTarget.y < 0) // up
		{
			anim.SetFloat ("direction", 0.66f);
		}
	}
}
