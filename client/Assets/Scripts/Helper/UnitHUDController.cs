using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHUDController : MonoBehaviour {
	private static UnitHUD hud;
	private static GameObject canvas;
	
	public static void Initialize() {
		canvas = GameObject.Find("GameHUDCanvas");
		if(!hud)
			hud = Resources.Load<UnitHUD>("Prefabs/UnitHUDParent");
	}
	
	public static UnitHUD CreateUnitHUD(string hp, string ar, string dmg, string pen, Unit unit) {
		UnitHUD instance = Instantiate(hud);
		
		instance.transform.SetParent(canvas.transform, false);
		
		instance.transform.SetAsFirstSibling();
		
		instance.hp.text =  hp;
		
		instance.ar.text = ar;
		
		instance.dmg.text = dmg;
		
		instance.pen.text = pen;
		
		instance.unit = unit;
		
		return instance;
	}
}
