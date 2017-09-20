using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour {

	[SerializeField] private TitleCtrl ctrl;

	[SerializeField] private Button select;
	private void Start()
	{
		ctrl = FindObjectOfType<TitleCtrl>();
		select.Select();
	}

	public void Battele(){
		ctrl.ToBattleScene();
	}

	public void StageSelect(){
		ctrl.ToStageSelect();
	}
}
