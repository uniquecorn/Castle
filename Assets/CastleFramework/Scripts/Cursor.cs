using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Cursor : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
		transform.position = CastleManager.tapPosition;
	}
}
