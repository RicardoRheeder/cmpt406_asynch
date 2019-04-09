using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour {
    public Text hp;
	public Text ar;
	public Text dmg;
	public Text pen;
	
	private bool hit = false;
	private int animCount = 0;
	private string targetHP;
	private string targetAR;
	private string targetDMG;
	private string targetPEN;
	
	public GameObject parent;
	public Unit unit;
	
	public void Update(){
		Vector2 screenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);
		parent.transform.position = screenPosition;
		if(targetHP != null && animCount > 3) {
			if(int.Parse(hp.GetComponent<Text>().text) < int.Parse(targetHP))
				hp.GetComponent<Text>().text = (int.Parse(hp.GetComponent<Text>().text) + 1).ToString();
			if(int.Parse(hp.GetComponent<Text>().text) > int.Parse(targetHP))
				hp.GetComponent<Text>().text = (int.Parse(hp.GetComponent<Text>().text) - 1).ToString();
			
			hit = true;
		}
		if(targetAR != null && animCount > 3) {
			if(int.Parse(ar.GetComponent<Text>().text) < int.Parse(targetAR))
				ar.GetComponent<Text>().text = (int.Parse(ar.GetComponent<Text>().text) + 1).ToString();
			if(int.Parse(ar.GetComponent<Text>().text) > int.Parse(targetAR))
				ar.GetComponent<Text>().text = (int.Parse(ar.GetComponent<Text>().text) - 1).ToString();
			
			hit = true;
		}
		if(targetDMG != null && animCount > 3) {
			if(int.Parse(dmg.GetComponent<Text>().text) < int.Parse(targetDMG))
				dmg.GetComponent<Text>().text = (int.Parse(dmg.GetComponent<Text>().text) + 1).ToString();
			if(int.Parse(dmg.GetComponent<Text>().text) > int.Parse(targetDMG))
				dmg.GetComponent<Text>().text = (int.Parse(dmg.GetComponent<Text>().text) - 1).ToString();
			
			hit = true;
		}
		if(targetPEN != null && animCount > 3) {
			if(int.Parse(pen.GetComponent<Text>().text) < int.Parse(targetPEN))
				pen.GetComponent<Text>().text = (int.Parse(pen.GetComponent<Text>().text) + 1).ToString();
			if(int.Parse(pen.GetComponent<Text>().text) > int.Parse(targetPEN))
				pen.GetComponent<Text>().text = (int.Parse(pen.GetComponent<Text>().text) - 1).ToString();
			
			hit = true;
		}
		
		animCount++;
		if(hit)
			animCount = 0;
		hit = false;
	}
	
	public void SetHPText(string text) {
		targetHP = text;
		
	}
	
	public void SetARText(string text) {
		targetAR = text;
	}
	
	public void SetDMGText(string text) {
		targetDMG = text;
	}
	
	public void SetPENText(string text) {
		targetPEN = text;
	}
	
	public void DestroyThis(){
		Destroy(gameObject);
	}
}
