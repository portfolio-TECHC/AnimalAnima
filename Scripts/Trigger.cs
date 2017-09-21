using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    [SerializeField]
    GameManager gameManager;

    public void StartTimer() {
        gameManager.StartTimer();
        gameManager.AbleRigidbody();
        Destroy(this.gameObject);
	}
}
