using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "ScriptableObjects/CreatePieceData")]
public class PieceData : ScriptableObject
{
    [field: SerializeField]
    public PieceType PieceType { get; private set; }
    [field: SerializeField]
    public CharacterType CharacterType { get; private set; }
}

public enum PieceType
{
    None = 0,
}

public enum CharacterType
{
    None,
    Attacker,
    Buffer,
    Support,
    Tank,
}
