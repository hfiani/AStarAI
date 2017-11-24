using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
	public Vector3 FinalTarget = Vector3.one * 99999;
	public bool randomTargets;
	public bool followTarget;
	public GameObject followTargetObj;

	[SerializeField] private float speed = 0.5f;

	Vector3 _position;
	Vector3 _finalFollowTarget;
	Animator anim;
	Stack roadToTake = new Stack();
	bool startMoving = false;
	Vector3 nextTarget = Vector3.one * 99999;
	float t;
	Canvas CanvasObj;
	Canvas Lose;
	Text ScoreFar;

	// Use this for initialization
	void Start ()
	{
		CanvasObj = GameObject.Find ("GameEnded").GetComponent<Canvas>();
		Lose = GameObject.Find ("Lose").GetComponent<Canvas>();
		ScoreFar = GameObject.FindGameObjectWithTag ("score_far").GetComponent<Text>();
		ScoreFar.text = "x0";

		_position = transform.position - MapCreate.PositionStarter;
		anim = GetComponent<Animator> ();

		if (followTarget)
		{
			followTargetObj = GameObject.FindGameObjectWithTag ("Player");
			_finalFollowTarget = followTargetObj.transform.position;
			FinalTarget = followTargetObj.transform.position;
		}
		else if (randomTargets)
		{
			FinalTarget = MapCreate.PositionStarter + (Vector3) MapCreate.availableSpotsEnemy [UnityEngine.Random.Range (0, MapCreate.availableSpotsEnemy.Count - 1)];
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// if no movement and no target
		// => not moving/reached destination
		if (!startMoving && FinalTarget == Vector3.one * 99999)
		{
			// check if the target has moved location
			// => follow him
			if (Vector3.Distance (followTargetObj.transform.position, _finalFollowTarget) > 0.001f)
			{
				_finalFollowTarget = followTargetObj.transform.position;
				FinalTarget = followTargetObj.transform.position;
			}
		}
		// if no movement and there is a target and did not reach location of target
		// => get path using A* and start moving
		if (!startMoving && FinalTarget != Vector3.one * 99999 && transform.position != FinalTarget)
		{
			roadToTake = AStarFunctions.AStar (_position, FinalTarget - MapCreate.PositionStarter);

			ScoreFar.text = "x" + roadToTake.Count;

			// reset few variables
			FinalTarget = Vector3.one * 99999;
			startMoving = true;

			nextDirection ();
			t = 0;
			anim.SetBool ("walk", true);
		}
	}

	void FixedUpdate()
	{
		// move only if we got path and we have to move
		if (startMoving)
		{
			int typeTile = MapCreate.Map [-(int)_position.y, (int)_position.x];
			t += Time.deltaTime * speed / MapCreate.TileCost[typeTile];
			transform.position = Vector3.Lerp (_position + MapCreate.PositionStarter, nextTarget, t);
			// reached the next block (next target)
			// => we have to recalculate the new path due to automatic follow
			if (Vector3.Distance(transform.position, nextTarget) < 0.001f)
			{
				t = 0;
				_position = nextTarget - MapCreate.PositionStarter;
				startMoving = false;
				anim.SetBool ("walk", false);
				roadToTake.Clear();
				// reached target
				// => player lost
				if (Vector3.Distance (transform.position, followTargetObj.transform.position) < 0.001f)
				{
					CanvasObj.enabled = true;
					Lose.enabled = true;
					GameObject.Find ("Score").GetComponent<Canvas> ().enabled = false;
				}
				// not reached player
				// => simulate a new target to recalculate the path
				else
				{
					_finalFollowTarget = followTargetObj.transform.position;
					FinalTarget = followTargetObj.transform.position;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		// player lost because enemy reached him
		if (c.tag.Equals ("Player"))
		{
			startMoving = false;
			nextTarget = Vector3.zero;
			FinalTarget = Vector3.one * 99999;
			anim.SetBool ("walk", false);
			roadToTake.Clear();
			CanvasObj.enabled = true;
			Lose.enabled = true;
			MapCreate.GameEnded = true;
		}
	}

	/// <summary>
	/// Gets the next direction and changes animation accordingly
	/// </summary>
	void nextDirection()
	{
		if (startMoving && roadToTake.Count > 0 && !MapCreate.GameEnded)
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
}
