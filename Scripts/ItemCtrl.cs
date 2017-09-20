using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour {

	/// <summary>
	/// アイテムの種類
	/// </summary>
	public enum ITEM_TYPE {
		NONE,
		/// <summary>
		/// ボム
		/// </summary>
		BOMB,

		/// <summary>
		/// タライ
		/// </summary>
		WASHTUB,

		/// <summary>
		/// お寿司
		/// </summary>
		SUSHI,

		/// <summary>
		/// 毛魂磁石
		/// </summary>
		MAGNET,

		/// <summary>
		/// 火炎瓶
		/// </summary>
		MOLOTOV_COCKTAIL,

		/// <summary>
		/// 毛忍
		/// </summary>
		NINJA
	}

	public enum DIRECTION{
		EAST = 0,
		WEST,
		SOUTH,
		NORTH
	}

	[SerializeField, EnumLabel(typeof(DIRECTION))]
	private List<GameObject> PopRangePoint;

	/// <summary>
	/// 毛魂各色のポイント
	/// </summary>
	[SerializeField, EnumLabel(typeof(Kedama.KEDAMA_TYPE))]
	public List<int> kedamaPoint;

	/// <summary>
	/// アイテムの生成間隔(秒)
	/// </summary>
	[SerializeField] private int itemPopTime;

	/// <summary>
	/// アイテムの生成間隔(秒)
	/// </summary>
	public int ItemPopTime{
		get{ return itemPopTime; }
		set{ itemPopTime = value < 0 ? 0 : value; }
	}

	/// <summary>
	/// 毛魂の生成間隔(秒)
	/// </summary>
	[SerializeField] private int kedamaPopTime;

	/// <summary>
	/// 毛魂の生成間隔(秒)
	/// </summary>
	public int KedamaPopTime
	{
		get { return kedamaPopTime; }
		set { kedamaPopTime = value < 0 ? 0 : value; }
	}

	/// <summary>
	/// 各アイテムの生成率
	/// </summary>
	[EnumLabel(typeof(ITEM_TYPE))]
	public List<int> ItemProbability;

	/// <summary>
	/// アイテムのプレファブ
	/// </summary>
	[EnumLabel(typeof(ITEM_TYPE)), Header("プレファブ")]
	public List<GameObject> ItemObjects;

	[EnumLabel(typeof(Kedama.KEDAMA_TYPE))]
	public List<GameObject> KedamaObjects;


	[SerializeField] private int sushiTime;
	public int SushiTime{
		get{ return sushiTime; }
	}

	[SerializeField] private int bombMinusScore;
	public int BombMinusScore{
		get{ return bombMinusScore; }
		set{ value = bombMinusScore; }
	}

	[SerializeField] private int washTubTime;
	public int WashTubTime{
		get{ return washTubTime; }
	}

	/// <summary>
	/// アイテム生成コルーチンの保管
	/// </summary>
	private IEnumerator popItem;

	/// <summary>
	/// 毛魂生成コルーチンの保管
	/// </summary>
	private IEnumerator popKedama;
	
	private int range;

    [SerializeField, EnumLabel(typeof(ITEM_TYPE))]
    public List<GameObject> effect;

	private void Start()
	{
		for(int i = 0;i < ItemProbability.Count; i++){
			range += ItemProbability[i];
		}
	}


	public Vector3 RandomPopPos()
	{
		var randPosX = UnityEngine.Random.Range(PopRangePoint[(int)DIRECTION.WEST].transform.position.x,
													PopRangePoint[(int)DIRECTION.EAST].transform.position.x);

		var randPosZ = UnityEngine.Random.Range(PopRangePoint[(int)DIRECTION.SOUTH].transform.position.z,
												PopRangePoint[(int)DIRECTION.NORTH].transform.position.z);

		return new Vector3(randPosX, 0f, randPosZ);
	}

	private Vector3 RandomPopRotation(){
		var randRotationY = UnityEngine.Random.Range(0, 360);
		return new Vector3(0f, randRotationY, 0f);
	}

	/// <summary>
	/// アイテム生成
	/// </summary>
	IEnumerator PopItem(){

		var wait = new WaitForSeconds((float)ItemPopTime);

		while (true)
		{
			//	指定秒毎に繰り返す
			yield return wait;

			//	当選アイテム
			GameObject popItemPrefab = null;

			//	抽選番号
			int randNum = UnityEngine.Random.Range(1, range + 1);
			
			//	確率の範囲
			int RangeOfProbability = 0;

			//	試行回数
			int i = 1;

			for (; i < ItemProbability.Count; i++){
				//	捜索範囲の追加
				RangeOfProbability += ItemProbability[i];

				//	抽選番号が抽選確立の範囲内か調べる
				if(randNum <= RangeOfProbability){
					//	範囲内なら当選アイテムを入れ、捜索を終了
					popItemPrefab = ItemObjects[i];
					break;
				}
			}
			

			//	当選したアイテムを生成する
			Instantiate(popItemPrefab, RandomPopPos(),Quaternion.identity);

		}
	}
	IEnumerator PopKedama(){
		var wait = new WaitForSeconds((float)KedamaPopTime);

		while (true)
		{
			//	指定秒毎に繰り返す
			yield return wait;


			//	アイテムを生成する
			Instantiate(KedamaObjects[(int)Kedama.KEDAMA_TYPE.BLUE], RandomPopPos(), Quaternion.identity);
			Instantiate(KedamaObjects[(int)Kedama.KEDAMA_TYPE.YELLOW], RandomPopPos(), Quaternion.identity);
			Instantiate(KedamaObjects[(int)Kedama.KEDAMA_TYPE.RED], RandomPopPos(), Quaternion.identity);
		}

	}


	public void StartPopItem(){
		popItem = PopItem();
		StartCoroutine(popItem);
	}

	public void StopPopItem(){
		StopCoroutine(popItem);
	}

	public void StartPopKedama()
	{
		popKedama = PopKedama();
		StartCoroutine(popKedama);
	}

	public void StopPopKedama()
	{
		StopCoroutine(popKedama);
	}

	//todo: アイテム挙動
}
