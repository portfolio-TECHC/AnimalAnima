using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour {

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

	//[SerializeField]
	//private Text scoreText;

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

	//todo: player移動

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

	[SerializeField]
	private int dir = 1;

    [SerializeField]
    private List<Image> scoreImg;

    [SerializeField]
    public Animation scoreAnimation;

	[SerializeField]
	private NumberSprite numberSprite;

	private void Start()
	{
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

		var buttonName = "Player" + (int)PlayerNum + "_";

		var leftFlag = Input.GetAxis(buttonName + getKeyName(KeyType.HORIZONTAL)) < -0.5f;
		var rightFlag = 0.5f < Input.GetAxis(buttonName + getKeyName(KeyType.HORIZONTAL));

		var playerRigidbody = this.GetComponent<Rigidbody>();

		//	アクセル
		if (Input.GetButton(buttonName + getKeyName(KeyType.ACCEL))) {
			playerRigidbody.AddForce(transform.forward * 17f * dir);
		}

		//	ブレーキ
		if (Input.GetButton(buttonName + getKeyName(KeyType.BRAKE)))
		{
			playerRigidbody.AddForce(transform.forward * -17f * dir);
		}

		if(rightFlag){
			this.transform.Rotate(new Vector3(0f, 2.5f * dir, 0f));
		}
		if (leftFlag)
		{
			this.transform.Rotate(new Vector3(0f, -2.5f * dir, 0f));
		}

		
		if(Input.GetKey(KeyCode.A)){

			this.transform.Rotate(new Vector3(0f, -2.5f * dir, 0f));
		}
		if (Input.GetKey(KeyCode.D))
		{
			this.transform.Rotate(new Vector3(0f, 2.5f * dir, 0f));
		}
		
	}
	

	private void FixedUpdate()
	{
		CheckKey();
		
	}


	//todo: アニメーション制御



	//todo:	スコア追加

	public void AddItem(ItemCtrl.ITEM_TYPE itemType){
		var itemCtrl = gameManager.GetComponent<ItemCtrl>();
		switch (itemType){
			
			case ItemCtrl.ITEM_TYPE.BOMB:
				{
					MinusScore(itemCtrl.BombMinusScore);
					var pos = gameObject.transform.position;

					var effect = Instantiate(itemCtrl.effect[(int)itemType], pos, Quaternion.identity);
					Destroy(effect, effect.GetComponentInChildren<ParticleSystem>().duration);
					
					break;
				}
			case ItemCtrl.ITEM_TYPE.WASHTUB:
				{
					StartCoroutine(WashTub(itemType, itemCtrl.WashTubTime));
					break;
				}
			case ItemCtrl.ITEM_TYPE.SUSHI:
				{
					StartCoroutine(Sushi(itemType, itemCtrl.SushiTime));
					break;
				}
			default: break;
		}
	}

	IEnumerator WashTub(ItemCtrl.ITEM_TYPE itemType, int time){
		dir = -1;
        GameObject effect = null;
        if (!ActivatingItems.Contains(itemType))
        {
            var ic = gameManager.GetComponent<ItemCtrl>();
            effect = Instantiate(ic.effect[(int)itemType]);
            effect.transform.SetParent(this.gameObject.transform, false);
        }

        ActivatingItems.Add(itemType);
        yield return new WaitForSeconds(time);
		dir = 1;
        
            Destroy(effect);

        ActivatingItems.Remove(itemType);
    }

	IEnumerator Sushi(ItemCtrl.ITEM_TYPE itemType, int time){
        GameObject effect = null;
        if (!ActivatingItems.Contains(itemType))
        {
            var ic = gameManager.GetComponent<ItemCtrl>();
            effect = Instantiate(ic.effect[(int)itemType]);
            effect.transform.SetParent(this.gameObject.transform, false);
        }
        ActivatingItems.Add(itemType);

		yield return new WaitForSeconds(time);
        Destroy(effect);

        ActivatingItems.Remove(itemType);
    }

	public void PlusScore(int point){

		//　お寿司発動中
		//	取得ポイントが2倍

		if (ActivatingItems != null)
		{
			if (ActivatingItems.Contains(ItemCtrl.ITEM_TYPE.SUSHI))
			{
				Score += point * 2;
			}else{
				Score += point;
			}
		}else{
			Score += point;
		}

		UpdateScoreText();
	}

	public void MinusScore(int point){
		Score -= point;
		UpdateScoreText();
	}

	public void UpdateScoreText(){
		//scoreText.text = Score.ToString();
        var gm = gameManager.GetComponent<GameManager>();
        int score_x00 = Score / 100;
        int score_0x0 = (Score - score_x00 * 100) / 10;
        int score_00x = Score - score_x00 * 100 - score_0x0 * 10;

        
        scoreImg[0].sprite = numberSprite.NumberSpriteList[score_x00];
        scoreImg[1].sprite = numberSprite.NumberSpriteList[score_0x0];
        scoreImg[2].sprite = numberSprite.NumberSpriteList[score_00x];
        


	}

}
