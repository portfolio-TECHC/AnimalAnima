using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleCtrl : MonoBehaviour {


	[SerializeField] private bool isStart = false;

	IEnumerator demo;

	public enum UI_TYPE{
		DEMO_UI,
		TITLE_UI,
		TITLE_UI_TITLE,
		TITLE_UI_PLAYER_SELECT,
		TITLE_UI_STAGE_INFO,
		ITEM_INFO_UI,
		ITEM_INFO_UI_KEDAMA,
		ITEM_INFO_UI_ITEM
	}

	[SerializeField, EnumLabel(typeof(UI_TYPE))]
	private List<GameObject> UIList;

	[SerializeField, EnumLabel(typeof(UI_TYPE))]
	private List<GameObject> UIActiveList;

	[SerializeField] private int toDemoTime;

	private void Start()
	{
		UIActiveList = new List<GameObject>(8);
		ChangeUI(UI_TYPE.TITLE_UI_TITLE);
		demo = TitleDemo();
		
		StartCoroutine(demo);

		
	}

	IEnumerator TitleDemo(){
		yield return new WaitForSeconds(toDemoTime);
		ChangeUI(UI_TYPE.DEMO_UI);
	}

	public void ChangeUI(UI_TYPE type){
		foreach (var go in UIActiveList)
		{
			Destroy(go);
		}
		UIActiveList.Clear();
		GameObject child = null;
		GameObject parent = null;
		switch (type){
			case UI_TYPE.DEMO_UI:
				UIActiveList.Add(Instantiate(UIList[(int)UI_TYPE.DEMO_UI]));
				break;
			case UI_TYPE.TITLE_UI:
				UIActiveList.Add(Instantiate(UIList[(int)UI_TYPE.TITLE_UI]));
				break;
			case UI_TYPE.TITLE_UI_TITLE:
				parent = Instantiate(UIList[(int)UI_TYPE.TITLE_UI]);
				child = Instantiate(UIList[(int)UI_TYPE.TITLE_UI_TITLE]);
				child.transform.SetParent(parent.transform, false);
				UIActiveList.Add(parent);
				UIActiveList.Add(child);

				break;
			case UI_TYPE.TITLE_UI_PLAYER_SELECT:
				parent = Instantiate(UIList[(int)UI_TYPE.TITLE_UI]);
				child = Instantiate(UIList[(int)UI_TYPE.TITLE_UI_PLAYER_SELECT]);
				child.transform.SetParent(parent.transform, false);
				UIActiveList.Add(parent);
				UIActiveList.Add(child);
				break;
			case UI_TYPE.TITLE_UI_STAGE_INFO:
				parent = Instantiate(UIList[(int)UI_TYPE.TITLE_UI]);
				child = Instantiate(UIList[(int)UI_TYPE.TITLE_UI_STAGE_INFO]);
				child.transform.SetParent(parent.transform, false);
				UIActiveList.Add(parent);
				UIActiveList.Add(child);
				break;
			case UI_TYPE.ITEM_INFO_UI:
				parent = Instantiate(UIList[(int)UI_TYPE.ITEM_INFO_UI]);
				UIActiveList.Add(parent);
				parent.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
				break;
			case UI_TYPE.ITEM_INFO_UI_KEDAMA:
				break;
			case UI_TYPE.ITEM_INFO_UI_ITEM:
				break;
			default:break;
		}
		
		
		
	}
	

	private void Update()
	{
		if (Input.anyKeyDown)
		{
			foreach (var go in UIActiveList)
			{
				if (go.name == UIList[(int)UI_TYPE.TITLE_UI_TITLE].name + "(Clone)")
				{
					isStart = true;
					StopCoroutine(demo);

					ChangeUI(UI_TYPE.TITLE_UI_PLAYER_SELECT);
				}
			}
		}
		

		if (!isStart && Input.anyKeyDown){
			StopCoroutine(demo);
			demo = TitleDemo();
			StartCoroutine(demo);
			ChangeUI(UI_TYPE.TITLE_UI_TITLE);
		}

		if(isStart && (Input.GetButton("Player1_Brake") || Input.GetButton("Player2_Brake"))){

			foreach (var go in UIActiveList)
			{
				if (go.name == UIList[(int)UI_TYPE.TITLE_UI_STAGE_INFO].name + "(Clone)")
				{
					ChangeUI(UI_TYPE.TITLE_UI_PLAYER_SELECT);
				}else if (go.name == UIList[(int)UI_TYPE.TITLE_UI_PLAYER_SELECT].name + "(Clone)"){
					ChangeUI(UI_TYPE.TITLE_UI_TITLE);
				}
			}
		}
		
	}

	public void ToStageSelect(){
		ChangeUI(UI_TYPE.TITLE_UI_STAGE_INFO);
	}
	public void ToBattleScene(){
		StartCoroutine(ToBattle());
	}

	IEnumerator ToBattle(){
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene("BatteleScene");
	}
}
