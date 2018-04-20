namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class CastleManager
	{
		public static CastleObject selectedObject, hoveredObject;
		public static Vector2 tapPosition;
		public static Collider2D[] colliderBuffer;
		static CastleObject focusedObject;
		static Plane inputPlane;
		static Vector3 worldTapPos;

		public enum HoverState
		{
			None,
			EnterHover,
			Hover,
			ExitHover
		}

		public enum SelectedState
		{
			None,
			Tap,
			Hold,
			Release
		}

		public enum CastleInputMode
		{
			SIMPLE,
			COMPLEX
		}

		public static CastleInputMode inputMode;
		
		/// <summary>
		/// Initialises your input;
		/// </summary>
		/// <param name="_inputMode">Input mode for checks. Simple for non tilted cameras, complex for cameras with angles.</param>
		public static void Init(CastleInputMode _inputMode = CastleInputMode.SIMPLE)
		{
			inputMode = _inputMode;
			switch(inputMode)
			{
				case CastleInputMode.SIMPLE:

					break;
				case CastleInputMode.COMPLEX:
					inputPlane = new Plane(-Vector3.forward, Vector3.zero);
					break;
			}
		}
		public static void SetInputPlane(Vector3 normal, Vector3 planePos)
		{
			inputPlane.SetNormalAndPosition(-normal, planePos);
		}
		/// <summary>
		/// Call this function using your game manager to handle touch input.
		/// </summary>
		public static void CastleUpdate()
		{
			switch (inputMode)
			{
				case CastleInputMode.SIMPLE:
					worldTapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					break;
				case CastleInputMode.COMPLEX:
					//inputPlane.SetNormalAndPosition(-Vector3.forward, Vector3.zero);
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					float hitdist = 0.0f;
					if (inputPlane.Raycast(ray, out hitdist))
					{
						worldTapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + (Vector3.forward * hitdist));
					}
					break;
			}
			tapPosition = new Vector2(worldTapPos.x, worldTapPos.y);
			colliderBuffer = Physics2D.OverlapPointAll(tapPosition);
			focusedObject = IsolateObject(colliderBuffer);

			Hover(focusedObject);
			Select(focusedObject);
		}

		static bool DetectObject(Collider2D[] _colls)
		{
			if (_colls.Length == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		static CastleObject IsolateObject(Collider2D[] _colls)
		{
			if (DetectObject(_colls))
			{
				return ClosestObject(_colls);
			}
			else
			{
				return null;
			}
		}

		static CastleObject ClosestObject(Collider2D[] _colls, bool excludeSelected = true)
		{
			float closestDist = 99;
			int chosenColl = 0;
			for (int i = 0; i < _colls.Length; i++)
			{
				if (_colls[i].transform.position.z < closestDist)
				{
					if (excludeSelected && selectedObject)
					{
						if (_colls[i] != selectedObject.coll)
						{
							closestDist = _colls[i].transform.position.z;
							chosenColl = i;
						}
					}
					else
					{
						closestDist = _colls[i].transform.position.z;
						chosenColl = i;
					}
				}
			}
			if (closestDist == 99)
			{
				return null;
			}
			else
			{
				return _colls[chosenColl].GetComponent<CastleObject>();
			}
		}

		public static void Drag(this CastleObject _object, float dragDelay = 10, bool instant = false)
		{
			if (instant)
			{
				_object.transform.position = Tools.Vec3RepZ(tapPosition, _object.transform.position.z);
			}
			else
			{
				_object.transform.position = Vector3.Lerp(_object.transform.position, Tools.Vec3RepZ(tapPosition, _object.transform.position.z), Time.deltaTime * dragDelay);
			}
		}
		public static void Unselect()
		{
			if (selectedObject)
			{
				selectedObject.Release();
				selectedObject = null;
			}
		}

		public static void Hover(CastleObject _object)
		{
			if (!_object)
			{
				if (hoveredObject)
				{
					hoveredObject.ExitHover();
					hoveredObject = null;
				}
			}
			else if (!hoveredObject)
			{
				hoveredObject = _object;
				hoveredObject.EnterHover();
			}
			else if (hoveredObject == _object)
			{
				hoveredObject.Hover();
			}
			else
			{
				hoveredObject.ExitHover();
				hoveredObject = _object;
				hoveredObject.EnterHover();
			}
		}

		public static void Select(CastleObject _object, bool _override = false)
		{
			if (!selectedObject && !_object)
			{
				return;
			}
			if (_override)
			{
				if (selectedObject)
				{
					selectedObject.Release();
				}
				selectedObject = _object;
				selectedObject.Tap();
				return;
			}
			if (Input.GetMouseButtonDown(0))
			{
				selectedObject = _object;
				selectedObject.Tap();
			}
			else if (Input.GetMouseButton(0))
			{
				if (selectedObject)
				{
					selectedObject.Hold();
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (selectedObject)
				{
					selectedObject.Release();
					selectedObject = null;
				}
			}
		}
	}
}
