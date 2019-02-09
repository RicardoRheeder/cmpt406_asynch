using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{

    private CardController deck;

    public Tilemap tilemap;

    private GameManager manager;


    void Awake()
    {

    }

    public PlayerController(GameManager manager, CardController deck) {
        this.deck = deck;
        this.manager = manager;
    }

    public void Initialize(GameManager manager, CardController deck)
    {
        this.deck = deck;
        this.manager = manager;
    }


    void Update()
    {
      
        InputController();
    }

    private void InputController()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);   

        //test
        if (Input.GetMouseButtonDown(0))
        {
            manager.GetUnitOnTile(tilePos);
            
            // print(tilePos);
          
        }
    }


}
