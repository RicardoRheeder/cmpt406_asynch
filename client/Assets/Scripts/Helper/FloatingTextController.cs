using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextController : MonoBehaviour {
    
	private static FloatingText popupText;
	private static GameObject canvas;
	
	public static void Initialize() {
		canvas = GameObject.Find("GameHUDCanvas");
		if(!popupText)
			popupText = Resources.Load<FloatingText>("Prefabs/FloatingTextParent");
	}
	
	public static void CreateFloatingText(string text, Transform location, bool isHeal) {
		Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
		
		FloatingText instance = Instantiate(popupText);
		
		if(isHeal)
			instance.animator.GetComponent<Text>().color = Color.green;
		
		instance.transform.SetParent(canvas.transform, false);
		
		instance.transform.position = screenPosition;
		
		instance.SetText(text);
	}
}
