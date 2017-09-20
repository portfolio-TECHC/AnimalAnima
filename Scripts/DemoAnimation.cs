using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoAnimation : MonoBehaviour {

	[SerializeField] public TitleCtrl ctrl;

	private void Start()
	{
		ctrl = FindObjectOfType<TitleCtrl>();
	}

	public void ChangeUI(TitleCtrl.UI_TYPE type){
		ctrl.ChangeUI(type);
	}
	
}
