using UnityEngine;

public class PieceController : MonoBehaviour
{
    [SerializeField]
    private PieceData _pieceData = default;

    private IPiece _movement = default;
}
