using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 毛魂（アニマ）
/// </summary>
public class Kedama : MonoBehaviour {

	/// <summary>
	/// アニマの色
	/// </summary>
	public enum KEDAMA_TYPE {
		BLUE,
		YELLOW,
		RED
	}

	/// <summary>
	/// アニマの色を選択
	/// </summary>
	public KEDAMA_TYPE KedamaType;

	[SerializeField] PlayerCtrl playerCtrl;
	[SerializeField] private GameObject gameManager;
	[SerializeField] private AudioClip SE;
	[SerializeField]
	private int point;

	private void Start()
	{
		gameManager = GameObject.FindWithTag("GameManager");
		point = gameManager.GetComponent<ItemCtrl>().kedamaPoint[(int)KedamaType];
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerCtrl = other.GetComponent<PlayerCtrl>();
			playerCtrl.PlusScore(point);
            playerCtrl.scoreAnimation.Play(getAnimationName());

			other.GetComponent<AudioSource>().PlayOneShot(SE);
			Destroy(this.gameObject);
		}
		else if(other.CompareTag("StageObject")){
			this.transform.position = gameManager.GetComponent<ItemCtrl>().RandomPopPos();
		}
	}

    private string getAnimationName()
    {
        string name = "";
        switch (KedamaType)
        {
            case KEDAMA_TYPE.BLUE: name = "ScoreGet_Blue"; break;
            case KEDAMA_TYPE.RED: name = "ScoreGet_Red"; break;
            case KEDAMA_TYPE.YELLOW: name = "ScoreGet_Yellow"; break;
        }
        return name;   
    }
}
