using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour {
    public Text hp;
	public Text ar;
	public Text dmg;
	public Text pen;
	
	public GameObject parent;
	public Unit unit;
	
	public void Update(){
		Vector2 screenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);
		parent.transform.position = screenPosition;
	}
	
	public void SetHPText(string text) {
		hp.GetComponent<Text>().text = text;
	}
	
	public void SetARText(string text) {
		ar.GetComponent<Text>().text = text;
	}
	
	public void SetDMGText(string text) {
		dmg.GetComponent<Text>().text = text;
	}
	
	public void SetPENText(string text) {
		pen.GetComponent<Text>().text = text;
	}
	
	public void DestroyThis(){
		Destroy(gameObject);
	}
}
