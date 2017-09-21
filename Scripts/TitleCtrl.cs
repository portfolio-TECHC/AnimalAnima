using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面コントローラー
/// </summary>
public class TitleCtrl : MonoBehaviour {

	//	スタートフラグ（タイトル画面で決定ボタンを押したか）
	[SerializeField] private bool isStart = false;

	//	デモの再生
	IEnumerator demo;

	//	UIのタイプ
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

	//	UIのプレファブ
	[SerializeField, EnumLabel(typeof(UI_TYPE))]
	private List<GameObject> UIList;

	//	アクティブなUIのリスト
	[SerializeField, EnumLabel(typeof(UI_TYPE))]
	private List<GameObject> UIActiveList;

	//	タイトルからデモ画面へ遷移する時間
	[SerializeField] private int toDemoTime;

	private void Start()
	{
		//	タイトル画面を出す
		ChangeUI(UI_TYPE.TITLE_UI_TITLE);
		//	タイトルデモのコルーチンを保持
		demo = TitleDemo();
		//	デモ再生
		StartCoroutine(demo);

		
	}

	/// <summary>
	/// デモ再生
	/// </summary>
	IEnumerator TitleDemo(){
		//	タイトルから設定時間待ち
		yield return new WaitForSeconds(toDemoTime);
		//	デモ画面を表示
		ChangeUI(UI_TYPE.DEMO_UI);
	}

	/// <summary>
	/// UI
	/// </summary>
	/// <param name="type"></param>
	public void ChangeUI(UI_TYPE type){
		//	表示中のUIをすべて削除
		foreach (var go in UIActiveList)
		{
			Destroy(go);
		}
		//	アクティブなUIリストをクリアに
		UIActiveList.Clear();
		
		//	親オブジェクト
		GameObject parent = null;
		//	子オブジェクト
		GameObject child = null;

		//	UIのタイプによって挙動を追加
		//hack: ウンコード
		//diary: 少しでもまともなUIに近づけたい（無断実装）明日締め切り……夜中3時……
		switch (type){
			case UI_TYPE.DEMO_UI:
				//	UIのアクティブリストに追加
				UIActiveList.Add(Instantiate(UIList[(int)UI_TYPE.DEMO_UI]));
				break;
			case UI_TYPE.TITLE_UI:
				UIActiveList.Add(Instantiate(UIList[(int)UI_TYPE.TITLE_UI]));
				break;
			case UI_TYPE.TITLE_UI_TITLE:
				//	タイトルUI（キャンバス）を親
				parent = Instantiate(UIList[(int)UI_TYPE.TITLE_UI]);
				//	タイトル画面を子に
				child = Instantiate(UIList[(int)UI_TYPE.TITLE_UI_TITLE]);
				//	親オブジェクトの子にする
				child.transform.SetParent(parent.transform, false);
				//	リストに追加
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

		//	タイトル画面で、1Pしかプレイヤー選択に進めないのは、
		//	お客様にコントローラーをお渡しする際に、どちらが1Pかこちら側で分かりやすくするため
		//	デモ画面の解除などで2Pが反応するのは接続確認のため
		//	分かりづらくてバグと勘違いしやすかったら申し訳ないです
		//	コントローラーの接続が切れてさしなおした場合、1P、2Pが入れ替わることがあるようです。

		//	スタートフラグが立っていない&&1P,2Pのどれかのボタンを押す
        if (!isStart && Input.anyKeyDown)
        {
			//	デモのコルーチンをやり直し、タイトル画面を表示
			//	(デモまでのタイマーをリセット)
            StopCoroutine(demo);
            demo = TitleDemo();
            StartCoroutine(demo);
            ChangeUI(UI_TYPE.TITLE_UI_TITLE);
        }

		//	1Pの決定ボタンが押されたら、プレイヤーセレクトを表示
        if (Input.GetButton("Player1_Accel") && !isStart)
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

		//	プレイヤーセレクト～のステップ時に、キャンセルボタンが押されたら、
		if(isStart && (Input.GetButton("Player1_Brake") || Input.GetButton("Player2_Brake"))){

			//	アクティブUIリストから
			foreach (var go in UIActiveList)
			{
				//	ステージ選択か検索、該当する場合は、プレイヤーセレクトに戻る
				if (go.name == UIList[(int)UI_TYPE.TITLE_UI_STAGE_INFO].name + "(Clone)")
				{
					ChangeUI(UI_TYPE.TITLE_UI_PLAYER_SELECT);
				}
				//	プレイヤーセレクトだった場合は、タイトルに戻る
				else if (go.name == UIList[(int)UI_TYPE.TITLE_UI_PLAYER_SELECT].name + "(Clone)"){
                    isStart = false;
					ChangeUI(UI_TYPE.TITLE_UI_TITLE);
				}
			}
		}
		
	}

	//	ステージセレクトへ移行
	public void ToStageSelect(){
		ChangeUI(UI_TYPE.TITLE_UI_STAGE_INFO);
	}
	//	バトルシーンへ移行
	public void ToBattleScene(){

        SceneManager.LoadScene("BatteleScene");
    }

}
