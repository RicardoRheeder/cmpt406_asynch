﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : INetwork{
    public bool EndTurn(EndTurnState state){return true;}
    public bool ForfeitGame(string id){return true;}
    public bool ReadyUnits(ReadyUnitsGameState state){return true;}
    public Tuple<bool, GameState> GetOldGamestate(string id, int turnCount) {return null;}
    public Tuple<bool, GameState> GetGamestate(string id){return new Tuple<bool, GameState>(true, null);}
    
    public PlayerMetadata GetUserInformation(){
        PlayerMetadata p = new PlayerMetadata {
            Username = "katlin"
        };

        return p;
    }
}
