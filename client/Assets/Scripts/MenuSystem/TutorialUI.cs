using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUI : MonoBehaviour {
    //a list of all of the tutorials
    public TutorialPrefab[] tutorialList;
    //the place the buttons will go
    public GameObject buttonList;
    //the button prefab
    public GameObject buttonPrefab;

    public GameObject tutImage;
    public GameObject tutText;

    private void Awake() {
        //loads all of the tutorials
        tutorialList = Resources.LoadAll<TutorialPrefab>("Tutorial");

        //for each tutorial, make a button
        foreach(TutorialPrefab t in tutorialList) {
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            
            //set the button text to the prefab's button text
            button.GetComponentInChildren<TextMeshProUGUI>().text = t.buttonText;

            //set the onClick()
            button.GetComponent<Button>().onClick.AddListener(() => ChangeTut(t));

            //add it to the button view
            button.transform.SetParent(buttonList.transform);
        }
    }

    //when a button is clicked change the image and text.
    //x is the index of the tutorial to show
    public void ChangeTut(TutorialPrefab prefab) {
        tutImage.GetComponentInChildren<Image>().sprite = prefab.prefabImage;
        tutText.GetComponentInChildren<TextMeshProUGUI>().text = prefab.prefabText;
    }
}
