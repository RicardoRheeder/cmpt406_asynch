using System.Runtime.Serialization;

//The data contract and Data member components of this class allow it to be converted to and from json using c#s built in stuff
//This class should only be used by the client API and the mockServer.
//It is left as a visible class instead of being an internal class so that both the "Client" module and the MockServer module can use it.
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
