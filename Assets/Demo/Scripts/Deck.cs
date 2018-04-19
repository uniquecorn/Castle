using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Deck : CastleObject
{
	public override void Tap()
	{
		base.Tap();
		CastleManager.Select(GameManager.instance.CreateCard(transform.position - Vector3.forward), true);
	}
	public override void Hold()
	{
		base.Hold();
	}
	public override void Release()
	{
		base.Release();
	}
}
