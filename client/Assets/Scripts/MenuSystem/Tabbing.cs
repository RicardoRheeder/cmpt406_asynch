using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tabbing : MonoBehaviour
{
    public Selectable nextField;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            nextField.Select();
    }
}
