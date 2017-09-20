using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class Result : MonoBehaviour {

	[SerializeField]
	private List<RankUI> rankUI;

	private List<PlayerCtrl.PLAYER_NUM> rankPlayerNumList;

	[SerializeField]
	Dictionary<PlayerCtrl.PLAYER_NUM, int> scoreList = new Dictionary<PlayerCtrl.PLAYER_NUM, int>();

	[SerializeField]
	private NumberSprite numberSprite;

	[SerializeField]
	private List<Sprite> playerNameSprite;

	private void Start()
	{
		//	急いで作ったので後で直す必要あり

		
		for(int i = 1; i <= 4; i++){
			if (PlayerPrefs.HasKey(Enum.GetName(typeof(PlayerCtrl.PLAYER_NUM), i) + "Score"))
			{
				scoreList[(PlayerCtrl.PLAYER_NUM)i] = PlayerPrefs.GetInt(Enum.GetName(typeof(PlayerCtrl.PLAYER_NUM), i) + "Score");

			}
		}

		var sorted = scoreList.OrderByDescending((x) => x.Value);

		int num = 0;
		foreach (var s in sorted){
			rankUI[num].PlayerName.sprite = playerNameSprite[(int)s.Key - 1];
			rankUI[num].SetScore(s.Value, numberSprite);
			num++;
		}

	}


	private void Update()
	{
		if (Input.anyKeyDown){

			PlayerPrefs.DeleteAll();
			SceneManager.LoadScene("TitleScene");
		}
	}

	[Serializable]
	class RankUI{
		[SerializeField]
		public Image PlayerName;

		[SerializeField]
		public List<Image> ScoreNum;


		public void SetScore(int score, NumberSprite number)
		{
			int score_x00 = score / 100;

			int score_0x0 = (score - score_x00 * 100) / 10;
			int score_00x = score - score_x00 * 100 - score_0x0 * 10;
			ScoreNum[0].sprite = number.NumberSpriteList[score_x00];
			ScoreNum[1].sprite = number.NumberSpriteList[score_0x0];
			ScoreNum[2].sprite = number.NumberSpriteList[score_00x];
		}
	}
}
