using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxStart : MonoBehaviour {
    private GameState state;
    private GameManager manager;
    private readonly string stateString ="{\"actions\":null,\"aliveUsers\":[\"katlin\"]," +
                                "\"boardId\":420,\"cards\":[{\"discardPile\":null,\"drawPile\":null,\"hand\":null," +
                                "\"owner\":\"katlin\"}],\"createdBy\":\"katlin\",\"forfeitTime\":-1,\"gameName\":\"lolol\"," +
                                "\"generals\":[{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":200,\"owner\":\"katlin\",\"unitType\":103,\"xPos\":-11," +
                                "\"yPos\":-6}],\"id\":\"42e0cea6-69b1-443e-95b5-d571243e7035\"," +
                                "\"initUnits\":[{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":100,\"owner\":\"katlin\"," +
                                "\"unitType\":1,\"xPos\":-13,\"yPos\":-6},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":100,\"owner\":\"katlin\",\"unitType\":1,\"xPos\":-15,\"yPos\":-6}," +
                                "{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0," +
                                "\"health\":75,\"owner\":\"katlin\",\"unitType\":6,\"xPos\":-14,\"yPos\":-4},{\"ability1CoolDown\":0," +
                                "\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":75,\"owner\":\"katlin\"," +
                                "\"unitType\":6,\"xPos\":-12,\"yPos\":-4},{\"ability1CoolDown\":0,\"ability1Duration\":0," +
                                "\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":85,\"owner\":\"katlin\"," +
                                "\"unitType\":2,\"xPos\":-13,\"yPos\":-2},{\"ability1CoolDown\":0,\"ability1Duration\":0," +
                                "\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":85,\"owner\":\"katlin\",\"unitType\":2," +
                                "\"xPos\":-13,\"yPos\":2},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":90,\"owner\":\"katlin\",\"unitType\":5,\"xPos\":-12,\"yPos\":4}," +
                                "{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0," +
                                "\"health\":90,\"owner\":\"katlin\",\"unitType\":5,\"xPos\":-14,\"yPos\":4},{\"ability1CoolDown\":0," +
                                "\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":80," +
                                "\"owner\":\"katlin\",\"unitType\":7,\"xPos\":-10,\"yPos\":6},{\"ability1CoolDown\":0," +
                                "\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":80," +
                                "\"owner\":\"katlin\",\"unitType\":7,\"xPos\":-11,\"yPos\":6}],\"isPublic\":false,\"maxUsers\":2," +
                                "\"readyUsers\":[\"katlin\"],\"spotsAvailable\":0,\"turnNumber\":0,\"turnTime\":-1," +
                                "\"units\":[{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":100,\"owner\":\"katlin\",\"unitType\":1,\"xPos\":-13," +
                                "\"yPos\":-6},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":100,\"owner\":\"katlin\",\"unitType\":1,\"xPos\":-15," +
                                "\"yPos\":-6},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":75,\"owner\":\"katlin\",\"unitType\":6,\"xPos\":-14," +
                                "\"yPos\":-4},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":75,\"owner\":\"katlin\",\"unitType\":6,\"xPos\":-12," +
                                "\"yPos\":-4},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":85,\"owner\":\"katlin\",\"unitType\":2,\"xPos\":-13," +
                                "\"yPos\":-2},{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0," +
                                "\"ability2Duration\":0,\"health\":85,\"owner\":\"katlin\",\"unitType\":2,\"xPos\":-13,\"yPos\":2}," +
                                "{\"ability1CoolDown\":0,\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0," +
                                "\"health\":90,\"owner\":\"katlin\",\"unitType\":5,\"xPos\":-12,\"yPos\":4},{\"ability1CoolDown\":0," +
                                "\"ability1Duration\":0,\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":90,\"owner\":\"katlin\"," +
                                "\"unitType\":5,\"xPos\":-14,\"yPos\":4},{\"ability1CoolDown\":0,\"ability1Duration\":0," +
                                "\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":80,\"owner\":\"katlin\"," +
                                "\"unitType\":7,\"xPos\":-10,\"yPos\":6},{\"ability1CoolDown\":0,\"ability1Duration\":0," +
                                "\"ability2CoolDown\":0,\"ability2Duration\":0,\"health\":80,\"owner\":\"katlin\"," +
                                "\"unitType\":7,\"xPos\":-11,\"yPos\":6}],\"users\":[\"katlin\"],\"usersTurn\":\"katlin\"}";
    
    public void StartSandbox() {
        state = JsonConversion.CreateFromJson<GameState>(stateString, typeof(GameState));
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        manager.StartSandbox(state);
    }
}
