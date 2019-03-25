using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{

    public string displayName;
    public string[] effects;
    public string description;
    public Sprite artwork;

    public CardFunction func;
    public UnitType type;

    public int cardCost;
}
