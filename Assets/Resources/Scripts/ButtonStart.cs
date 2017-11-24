using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour
{
	[SerializeField] private GameObject Reference;

	public void MouseDown()
	{
		transform.localPosition = Reference.transform.localPosition;
	}

	public void MouseUp()
	{
		transform.localPosition = Reference.transform.localPosition + new Vector3(0, +5, +0);
	}

	public void MouseClick()
	{
		SceneManager.LoadScene ("Game");
	}
}
