using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToPsEcReT : MonoBehaviour
{
    
	int code = 0;
	GameObject i;
	TMP_Text title;
	
	
	void Start(){
		i = GameObject.Find("Secret");
		title = GameObject.Find("GameTitle").GetComponent<TMP_Text>();
	}
	
	
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.UpArrow) && code < 2){
		    code++;
		}
		if(Input.GetKeyUp(KeyCode.DownArrow) && code > 1 && code < 4){
			code++;
		}
		if(Input.GetKeyUp(KeyCode.LeftArrow) && (code == 4 || code == 6)){
			code++;
		}
		if(Input.GetKeyUp(KeyCode.RightArrow) && (code == 5 || code == 7)){
			code++;
		}
		if(Input.GetKeyUp(KeyCode.B) && code == 8){
			code++;
		}
		if(Input.GetKeyUp(KeyCode.A) && code == 9){
			code++;
		}
		if(code == 10){
			Debug.Log("YOU WIN");
			title.SetText("Cards & Chungus");
			code = 0;
			i.transform.SetSiblingIndex(2);
		}
    }
}
