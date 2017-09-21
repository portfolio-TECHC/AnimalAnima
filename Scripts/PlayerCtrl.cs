using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのコントローラー操作
/// アイテムの挙動、スコアの保持追加
/// </summary>
public class PlayerCtrl : MonoBehaviour {

	/// <summary>
	/// ゲームマネージャーの保持
	/// </summary>
	[SerializeField] private GameObject gameManager;

	/// <summary>
	/// スコア
	/// </summary>
	[SerializeField]
	private int score;

	/// <summary>
	/// スコア(0以上)
	/// </summary>
	public int Score {
		get { return score; }
		set { score = (score + value) < 0 ? 0 : value; }
	}

	/// <summary>
	/// プレイヤー番号
	/// </summary>
	public enum PLAYER_NUM {
		PLAYER_01 = 1,
		PLAYER_02,
		PLAYER_03,
		PLAYER_04
	}

	/// <summary>
	/// プレイヤー番号
	/// </summary>
	public PLAYER_NUM playerNum;

	/// <summary>
	/// プレイヤー番号(Max:4)
	/// </summary>
	public PLAYER_NUM PlayerNum {
		get { return playerNum; }
		private set { playerNum = value; }
	}

	/// <summary>
	///	発動中のアイテム
	/// </summary>
	[SerializeField]
	private List<ItemCtrl.ITEM_TYPE> activatingItems;

	/// <summary>
	///	発動中のアイテム
	/// </summary>
	public List<ItemCtrl.ITEM_TYPE> ActivatingItems
	{
		get{ return activatingItems; }
		set{ activatingItems = value; }
	}

	/// <summary>
	/// ストックされているアイテム
	/// </summary>
	public List<ItemCtrl.ITEM_TYPE> StockedItems;

	/// <summary>
	///	ボタン名リスト
	/// </summary>
	private string[] KeyNameStr = {
		"Accel",
		"Brake",
		"Horizontal",
		"Item"
	};

	/// <summary>
	/// ボタンタイプ
	/// </summary>
	public enum KeyType
	{
		ACCEL = 0,
		BRAKE,
		HORIZONTAL,
		ITEM
	}

	/// <summary>
	/// 移動方向（混乱時に反転させるため）
	/// </summary>
	[SerializeField]
	private int dir = 1;

	/// <summary>
	/// スコアのImageの保持（３桁）
	/// </summary>
    [SerializeField]
    private List<Image> scoreImg;

	/// <summary>
	/// スコア文字のアニメーション
	/// </summary>
    [SerializeField]
    public Animation scoreAnimation;

	/// <summary>
	/// 数字の画像
	/// </summary>
	[SerializeField]
	private NumberSprite numberSprite;

	/// <summary>
	/// 曲がる速さ
	/// </summary>
    private Vector3 eulerAngleVelocity;

    private void Start()
	{
		//	タグからGemeManagerを検索
		gameManager = GameObject.FindWithTag("GameManager");
	}

	/// <summary>
	/// ボタン名取得
	/// </summary>
	/// <param name="type">ボタンタイプ</param>
	/// <returns>ボタン名</returns>
	public string getKeyName(KeyType type)
	{
		return KeyNameStr[(int)type];
	}

	/// <summary>
	/// キーのチェック
	/// </summary>
	public void CheckKey() {

		//	何Pか
		var buttonName = "Player" + (int)PlayerNum + "_";

		//	曲がったと判定するためのアナログスティックの閾値を決める
		var leftFlag = Input.GetAxis(buttonName + getKeyName(KeyType.HORIZONTAL)) < -0.5f;
		var rightFlag = 0.5f < Input.GetAxis(buttonName + getKeyName(KeyType.HORIZONTAL));

		//	プレイヤーのリジッドボディ
		var playerRigidbody = this.GetComponent<Rigidbody>();
        


		//	アクセル
		if (Input.GetButton(buttonName + getKeyName(KeyType.ACCEL))) {
			//	正面に力を加える
			playerRigidbody.AddForce(transform.forward * 20f * dir);
		}

		//	ブレーキ
		if (Input.GetButton(buttonName + getKeyName(KeyType.BRAKE)))
		{
			//	摩擦を増やして減速
            playerRigidbody.drag = 0.7f;
            
        }else
        {
			//	ブレーキを話したら、摩擦を元に戻す
            playerRigidbody.drag = 0.2f;
        }

		//	曲がる向き、速さを決める（混乱状態でdir=-1で方向が反転する）
		//	アナログスティックの右入力
		if (rightFlag){
            eulerAngleVelocity = new Vector3(0f, 50f * dir, 0f);
        }

		//	アナログスティックの左入力
		if (leftFlag){
            eulerAngleVelocity = new Vector3(0f, -50f * dir, 0f);
        }
        
		//	両方の入力がない時には速度を０に
		if(!leftFlag && !rightFlag){
            eulerAngleVelocity = new Vector3(0f, 0f, 0f);
        }
        
		//	プレイヤーを回転させる
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        playerRigidbody.MoveRotation(playerRigidbody.rotation * deltaRotation);
		
	}

    private void FixedUpdate()
	{
		//	キーのチェック
        CheckKey();
		
	}


	//todo: アニメーション制御
	//todo: アニメーションのデータを揃える
	//diary: アニメーションデータとモデル下さい……

	/// <summary>
	/// アイテムをプレイヤーに付与する
	/// </summary>
	/// <param name="itemType">アイテムの種類</param>
	public void AddItem(ItemCtrl.ITEM_TYPE itemType)
	{

		//	アイテムコントローラーを取得
		var itemCtrl = gameManager.GetComponent<ItemCtrl>();

		//	アイテムによって挙動を追加
		switch (itemType)
		{
			case ItemCtrl.ITEM_TYPE.BOMB:
				{
					//	スコアをマイナス
					MinusScore(itemCtrl.BombMinusScore);

					//	取得したポジションに爆発エフェクトを生成
					var pos = gameObject.transform.position;
					var effect = Instantiate(itemCtrl.effect[(int)itemType], pos, Quaternion.identity);
					//	再生時間が終わったら、削除
					Destroy(effect, effect.GetComponentInChildren<ParticleSystem>().duration);

					break;
				}
			case ItemCtrl.ITEM_TYPE.WASHTUB:
				{
					//	一定時間効果が続くのでコルーチンへ
					StartCoroutine(WashTub(itemType, itemCtrl.WashTubTime));
					break;
				}
			case ItemCtrl.ITEM_TYPE.SUSHI:
				{
					//	一定時間効果が続くのでコルーチンへ
					StartCoroutine(Sushi(itemType, itemCtrl.SushiTime));
					break;
				}
			default: break;
		}
	}

	//	タライ
	IEnumerator WashTub(ItemCtrl.ITEM_TYPE itemType, int time){
		//	コントローラーの左右、進行方向を逆にする
		dir = -1;

		//　	エフェクト保持用
        GameObject effect = null;

		//	効果中のアイテムリストにタライがなければ
        if (!ActivatingItems.Contains(itemType))
        {
			//	アイテムコントローラーを取得
            var ic = gameManager.GetComponent<ItemCtrl>();
			//	アイテムに設定したエフェクトを生成
            effect = Instantiate(ic.effect[(int)itemType]);
			//	エフェクトをプレイヤーの子オブジェクトにして、プレイヤーに追従
            effect.transform.SetParent(this.gameObject.transform, false);
        }

		//	効果中のアイテムリストに追加
        ActivatingItems.Add(itemType);

		//	設定時間待つ
        yield return new WaitForSeconds(time);
		//	方向をもとに戻す
		dir = 1;
        
		//	エフェクトを削除
        Destroy(effect);
		//	効果アイテムリストからこのタライを削除
        ActivatingItems.Remove(itemType);
    }

	//	寿司
	IEnumerator Sushi(ItemCtrl.ITEM_TYPE itemType, int time)
	{
		//　	エフェクト保持用
		GameObject effect = null;

		//	効果中のアイテムリストに寿司がなければ
		if (!ActivatingItems.Contains(itemType))
        {
			//	アイテムコントローラーを取得
			var ic = gameManager.GetComponent<ItemCtrl>();
			//	アイテムに設定したエフェクトを生成
			effect = Instantiate(ic.effect[(int)itemType]);
			//	エフェクトをプレイヤーの子オブジェクトにして、プレイヤーに追従
			effect.transform.SetParent(this.gameObject.transform, false);
        }
		//	効果中のアイテムリストに追加
		ActivatingItems.Add(itemType);
		//	設定時間待つ
		yield return new WaitForSeconds(time);
		//	エフェクトを削除
		Destroy(effect);
		//	効果アイテムリストからこの寿司を削除
		ActivatingItems.Remove(itemType);
    }

	/// <summary>
	/// ポイント加算
	/// </summary>
	/// <param name="point"></param>
	public void PlusScore(int point){

		//　お寿司発動中
		//	取得ポイントが2倍

		//	効果中のアイテムがあるか
		if (ActivatingItems != null)
		{
			//	寿司が発動中なら
			if (ActivatingItems.Contains(ItemCtrl.ITEM_TYPE.SUSHI))
			{
				//	スコアを2倍にして、スコアに追加
				Score += point * 2;
			}else{
				//	寿司でないなら、通常のスコアを追加
				Score += point;
			}
		}else{
			//	通常のスコアを追加
			Score += point;
		}

		//	スコアテキストを更新
		UpdateScoreText();
	}

	//	スコア減算
	public void MinusScore(int point){
		//	スコアを引き
		Score -= point;
		//	スコアテキストを更新
		UpdateScoreText();
	}

	/// <summary>
	/// スコアテキスト更新
	/// </summary>
	public void UpdateScoreText(){
		//	ゲームマネージャーを取得
        var gm = gameManager.GetComponent<GameManager>();

		//	現在のスコアの
		//	百の位
        int score_x00 = Score / 100;
		//	十の位
        int score_0x0 = (Score - score_x00 * 100) / 10;
		//	一の位
        int score_00x = Score - score_x00 * 100 - score_0x0 * 10;

		//	それぞれの数値に対応した画像に張り替え
        scoreImg[0].sprite = numberSprite.NumberSpriteList[score_x00];
        scoreImg[1].sprite = numberSprite.NumberSpriteList[score_0x0];
        scoreImg[2].sprite = numberSprite.NumberSpriteList[score_00x];
	}
}
