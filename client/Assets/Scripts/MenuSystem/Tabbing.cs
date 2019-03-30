using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tabbing : MonoBehaviour {

    public bool startSelected = false;
    public Selectable self;
    public Selectable nextField;
    public Button button;

    public bool autoHitButton = false;

    public void Start(){
        if(startSelected) {
            self.Select();
        }
    }
    
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Tab) && nextField != null) {
            if (!autoHitButton) {
                nextField.Select();
            }
            else {
                ((Button)nextField).onClick.Invoke();
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) && button != null)
            button.onClick.Invoke();
    }
}
