using System.Runtime.Serialization;

//The data contract and Data member components of this class allow it to be converted to and from json using c#s built in stuff
[DataContract]
public class Credentials {

    [DataMember]
    private string username;

    [DataMember]
    private string password;

    public Credentials(string username, string password) {
        this.username = username;
        this.password = password;
    }
}
