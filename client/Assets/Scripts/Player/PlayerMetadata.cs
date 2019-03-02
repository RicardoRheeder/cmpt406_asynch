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

    //Set is public because the player metadata is serialized without that information, so it has
    //to be set from the client
    public string Username { get; set; }

    [DataMember(Name = "friends")]
    public List<string> Friends { get; private set; }

    [DataMember(Name = "activeGames")]
    public List<string> ActiveGames { get; private set; }

    [DataMember(Name = "pendingPrivateGames")]
    public List<string> PendingPrivateGames { get; private set; }

    [DataMember(Name = "pendingPublicGames")]
    public List<string> PendingPublicGames { get; private set; }

    [DataMember(Name = "completedGames")]
    public List<string> CompletedGames { get; private set; }

    [DataMember(Name = "armyPresets")]
    public List<ArmyPreset> ArmyPresets { get; private set; }

    public override string ToString() {
        return JsonConversion.ConvertObjectToJson(this);
    }

    //This function will be run after the class is serialized from the JSON string
    //you can use this to set default values, or ensure values are within some valid context
    [OnDeserialized]
    public void OnDeserialized(StreamingContext c) {
        if(Friends == null) {
            Friends = new List<string>();
        }
        if(ActiveGames == null) {
            ActiveGames = new List<string>();
        }
        if(PendingPrivateGames == null) {
            PendingPrivateGames = new List<string>();
        }
        if(PendingPublicGames == null) {
            PendingPublicGames = new List<string>();
        }
        if(CompletedGames == null) {
            CompletedGames = new List<string>();
        }
        if (ArmyPresets == null) {
            ArmyPresets = new List<ArmyPreset>();
        }
        else {
            //This is where the army builder cache is made.
            for(int i = 0; i < ArmyPresets.Count; i++) {
                ArmyBuilder.AddPreset(ArmyPresets[i].Name, ArmyPresets[i]);
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
