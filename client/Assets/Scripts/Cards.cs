using System.Runtime.Serialization;

[DataContract]
public class Cards {

    [DataMember]
    private string[] hand;

    [DataMember]
    private string[] deck;

    [DataMember]
    private string[] discard;

    public Cards(string[] hand, string[] deck, string[] discard) {
        this.hand = hand;
        this.deck = deck;
        this.discard = discard;
    }
}
