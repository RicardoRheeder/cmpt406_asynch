﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetwork {
  bool EndTurn(EndTurnState state);
  bool ForfeitGame(string id);
  bool ReadyUnits(ReadyUnitsGameState state);
  Tuple<bool, GameState> GetGamestate(string id);
  Tuple<bool, GameState> GetOldGamestate(string id, int turnCount);
  PlayerMetadata GetUserInformation();
}
