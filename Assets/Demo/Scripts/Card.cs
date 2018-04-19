using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Card : CastleObject
{
	public enum CardState
	{
		FREE,
		EQUIPPED
	}
	public CardState state;
	public Slot slot;
	public override void Tap()
	{
		if (state == CardState.EQUIPPED)
		{
			slot.Remove();
			slot = null;
		}
		state = CardState.FREE;
		base.Tap();
	}

	public override void Hold()
	{
		base.Hold();
		if (CastleManager.hoveredObject is Slot && ((Slot)CastleManager.hoveredObject).slotState == Slot.SlotState.IDLE)
		{
			transform.position = Vector3.Lerp(transform.position,Tools.Vec3RepZ(CastleManager.hoveredObject.transform.position, transform.position.z),Time.deltaTime * 20);
		}
		else if(CastleManager.hoveredObject is Deck)
		{
			transform.position = Vector3.Lerp(transform.position, Tools.Vec3RepZ(CastleManager.hoveredObject.transform.position, transform.position.z), Time.deltaTime * 20);
		}
		else
		{
			this.Drag();
		}
	}
	public override void Release()
	{
		if (CastleManager.hoveredObject is Deck)
		{
			Destroy(gameObject);
		}
		else if(CastleManager.hoveredObject is Slot && ((Slot)CastleManager.hoveredObject).slotState == Slot.SlotState.IDLE)
		{
			transform.position = Tools.Vec3RepZ(CastleManager.hoveredObject.transform.position, transform.position.z);
			((Slot)CastleManager.hoveredObject).Equip(this);
			state = CardState.EQUIPPED;
		}
		base.Release();
	}
	private void Update()
	{
		switch(state)
		{
			case CardState.FREE:
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10);
				break;
			case CardState.EQUIPPED:
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0),Time.deltaTime * 10);
				break;
		}
	}
}
