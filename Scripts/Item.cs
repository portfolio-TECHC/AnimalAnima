using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	[SerializeField] public ItemCtrl.ITEM_TYPE ItemType;
	[SerializeField] private PlayerCtrl playerCtrl;

	[SerializeField] private AudioClip SE;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerCtrl = other.GetComponent<PlayerCtrl>();
			playerCtrl.AddItem(ItemType);

			other.GetComponent<AudioSource>().PlayOneShot(SE);
			Destroy(this.gameObject);
		}
	}
}
