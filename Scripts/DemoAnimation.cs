using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルデモ
/// </summary>
public class DemoAnimation : MonoBehaviour {

	//	タイトルコントローラー
	[SerializeField] public TitleCtrl ctrl;

	private void Start()
	{
		ctrl = FindObjectOfType<TitleCtrl>();
	}

	/// <summary>
	/// UIの張り替え
	/// </summary>
	/// <param name="type">UIタイプ</param>
	public void ChangeUI(TitleCtrl.UI_TYPE type){
		ctrl.ChangeUI(type);
	}
	
}
