using System.Collections.Generic;
using System.Runtime.Serialization;

//This class represents the metadata for the player, which includes the following:
//  friends: a list of the usernames the player has on their friends list
//  activeGames: a list of games that the user is currently in
//  pendingPrivateGames: a list of private games that the user has either created or has been invited to
//  pendingPublicGames: a list of public games that the user has either created or has been invited to
//  completedGames: a list of games that the user was a part of but they have been completed.
[DataContract]
public class PlayerMetadata {

    [DataMember(Name = "freinds")]
    public List<string> friends = new List<string>();

    [DataMember]
    public List<string> activeGames = new List<string>();

    [DataMember]
    public List<string> pendingPrivateGames = new List<string>();

    [DataMember]
    public List<string> pendingPublicGames = new List<string>();

    [DataMember]
    public List<string> completedGames = new List<string>();

    [DataMember]
    public List<ArmyPreset> armyPresets = new List<ArmyPreset>();

    public override string ToString() {
        return JsonConversion.ConvertObjectToJson(typeof(PlayerMetadata), this);
    }

    //This function will be run after the class is serialized from the JSON string
    //you can use this to set default values, or ensure values are within some valid context
    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        if(friends == null) {
            friends = new List<string>();
        }
        if(activeGames == null) {
            activeGames = new List<string>();
        }
        if(pendingPrivateGames == null) {
            pendingPrivateGames = new List<string>();
        }
        if(pendingPublicGames == null) {
            pendingPublicGames = new List<string>();
        }
        if(completedGames == null) {
            completedGames = new List<string>();
        }
        if (armyPresets == null) {
            armyPresets = new List<ArmyPreset>();
        }
        else {
            //This is where the army builder cache is made.
            for(int i = 0; i < armyPresets.Count; i++) {
                ArmyBuilder.AddPreset(armyPresets[i].presetName, armyPresets[i]);
            }
        }
        ArmyBuilder.InsertDefaultPresets();
    }
}

//This looks really hacky, but we are doing it so we can convert metadata components to json string
[DataContract]
public class GameIds {
    [DataMember]
    private List<string> gameIds;

    public GameIds(List<string> data) {
        gameIds = data;
    }

    public GameIds(List<string> publicData, List<string> privateData) {
        gameIds = new List<string>();
        gameIds.AddRange(privateData);
        gameIds.AddRange(publicData);
    }
}
