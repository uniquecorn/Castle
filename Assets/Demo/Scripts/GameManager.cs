using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public GameObject cardPrefab;
	private void Awake()
	{
		instance = this;
	}
	// Use this for initialization
	void Start ()
	{
		CastleManager.Init();
	}
	
	public Card CreateCard(Vector3 positon)
	{
		return Instantiate(cardPrefab, positon, Quaternion.identity).GetComponent<Card>();
	}

	// Update is called once per frame
	void Update ()
	{
		CastleManager.CastleUpdate();
	}
}
