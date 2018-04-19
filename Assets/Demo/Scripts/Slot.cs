using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Castle;

public class Slot : CastleObject
{	
	public enum SlotState
	{
		IDLE,
		EQUIPPED
	}
	public SlotState slotState;

	public Card card;

	public void Equip(Card _card)
	{
		card = _card;
		card.slot = this;
		slotState = SlotState.EQUIPPED;
	}
	public void Remove()
	{
		card.slot = null;
		card = null;
		slotState = SlotState.IDLE;
	}
}
