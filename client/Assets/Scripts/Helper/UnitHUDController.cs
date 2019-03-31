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
		
		instance.SetHPText(hp);
		
		instance.SetARText(ar);
		
		instance.SetDMGText(dmg);
		
		instance.SetPENText(pen);
		
		instance.unit = unit;
		
		return instance;
	}
}
