using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable 649
public class TutorialUI : MonoBehaviour {

    //a list of all of the tutorials
    private TutorialPrefab[] tutorialList;

    [SerializeField]
    private GameObject buttonContainer;

    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private GameObject tutImage;

    [SerializeField]
    private GameObject tutText;

    public void Awake() {
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
            button.transform.SetParent(buttonContainer.transform);
        }
    }

    //when a button is clicked change the image and text.
    //x is the index of the tutorial to show
    public void ChangeTut(TutorialPrefab prefab) {
        GameObject.Find("Canvas").GetComponent<MainMenu>().SetTutorialMenuState(tutWindowState: true);
        tutImage.GetComponentInChildren<Image>().sprite = prefab.prefabImage;
        tutText.GetComponentInChildren<TextMeshProUGUI>().text = prefab.prefabText;
    }
}
