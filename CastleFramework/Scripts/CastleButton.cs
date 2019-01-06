namespace Castle
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;

	public class CastleButton : CastleObject
	{
		public Animator anim;
		public UnityEvent onClick;

		// Use this for initialization
		public override void EnterHover()
		{
			base.EnterHover();
			//anim.SetTrigger("EnterHover");
		}

		public override void Hover()
		{
			base.Hover();
		}

		public override void ExitHover()
		{
			base.ExitHover();
			//anim.SetTrigger("ExitHover");
		}

		public override void Tap()
		{
			base.Tap();
			//anim.SetTrigger("Tap");
			//TouchManager.Unselect();
		}

		public override void Release()
		{
			base.Release();
			onClick.Invoke();
			//anim.SetTrigger("Release");
		}
	}
}
