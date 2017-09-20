using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{

	[SerializeField] private ItemCtrl itemCtrl;
    [SerializeField]
    public GameObject UICanvas;
	[SerializeField] private int defaultTime_M;
	[SerializeField] private int defaultTime_S;
	[SerializeField] private GameObject FinishImage;
	[SerializeField] private List<PlayerCtrl> players;

	[SerializeField] private NumberSprite numberSpr;
    [SerializeField] private List<Image> timerImg;
    [SerializeField]
    private Animation timerAnimation;


	private void Start()
	{
		FinishImage.SetActive(false);

		itemCtrl.StartPopItem();
		itemCtrl.StartPopKedama();
        //UICanvas.SetActive(false);
        TimerSet(defaultTime_M * 60 + defaultTime_S);
		StartTimer();
	}

	//todo: ゲーム状態の管理

	//todo: タイマー

    void TimerSet(int time)
    {
        int m = time / 60;
        int s = time % 60;
        timerImg[0].sprite = numberSpr.NumberSpriteList[m / 10];
        timerImg[1].sprite = numberSpr.NumberSpriteList[m % 10];
        timerImg[2].sprite = numberSpr.NumberSpriteList[s / 10];
        timerImg[3].sprite = numberSpr.NumberSpriteList[s % 10];
    }

	IEnumerator Timer()
	{
		int time = defaultTime_S + defaultTime_M * 60;

        timerAnimation.Play("Timer_Idle");

		var wait = new WaitForSeconds(1f);

		while (true)
		{
			if (time == 0) break;

			yield return wait;

            timerAnimation.Stop();
            

			time--;

            TimerSet(time);

            if (time <= 10){
                timerAnimation.Play("Timer_Countdown");
            }
       

        }

        TimerSet(0);
        yield return new WaitForSeconds(1f);

        timerAnimation.Play("Timer_Finish");

        TimeUp();
	}

    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

	private void TimeUp(){
		Debug.Log("TimeUp");
		FinishImage.SetActive(true);
		itemCtrl.StopPopItem();
		itemCtrl.StopPopKedama();

		for (int PlayerNum = 1; PlayerNum < players.Count + 1; PlayerNum++){
			var saveKey = Enum.GetName(typeof(PlayerCtrl.PLAYER_NUM), PlayerNum) + "Score";
			var data = players[PlayerNum-1].Score;
			Debug.Log(saveKey + ":" + data);
			PlayerPrefs.SetInt(saveKey, data);
			PlayerPrefs.Save();
		}


		StartCoroutine(ToResultScene());
	}

	public IEnumerator ToResultScene(){
		yield return new WaitForSeconds(3f);
		SceneManager.LoadScene("ResultScene");
	}

	//todo: 各種設定

	//todo: UIの更新


}
