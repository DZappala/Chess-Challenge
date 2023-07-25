using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

// ReSharper disable once CheckNamespace
public class MyBot : IChessBot
{
	private int _pawnValue;
	private int _knightValue;
	private int _bishopValue;
	private int _rookValue;
	private int _queenValue;
	private int _kingValue;
	private int _depth;
	private ulong[] _bitboards;

	public MyBot()
	{
		_depth = 5;
		_pawnValue = 100;
		_knightValue = 320;
		_bishopValue = 330;
		_rookValue = 500;
		_queenValue = 900;
		_kingValue = 20000;
		_bitboards = new ulong[12];
	}

	public Move Think(Board board, Timer timer)
	{
		Move[] moves = board.GetLegalMoves();
		switch (moves.Length)
		{
			case 0:
				return new();
			case 1:
				return moves[0];
		}

		Move bestMove = moves[0];

		return bestMove;
	}

	public int Evaluate()
	{
		return 0;
	}

	public int Who2Move(Board board)
	{
		return board.IsWhiteToMove ? 1 : -1;
	}


	public int MaterialWeight(Board board, bool white)
	{
		return board.GetAllPieceLists().Where(pieceList => white == pieceList.IsWhitePieceList).SelectMany(pieceList => pieceList).Sum(GetPieceValue);
	}

	public int GetPieceValue(Piece piece)
	{
		return piece.PieceType switch
		{
			PieceType.Pawn => _pawnValue,
			PieceType.Knight => _knightValue,
			PieceType.Bishop => _bishopValue,
			PieceType.Rook => _rookValue,
			PieceType.Queen => _queenValue,
			PieceType.King => _kingValue,
			PieceType.None => 0,
			_ => 0,
		};
	}

	public bool IsEndGame(Board board)
	{
		if (board.GetPieceList(PieceType.Queen, true).Count == 0 &&
			board.GetPieceList(PieceType.Queen, false).Count == 0)
			return true;
		return NumPieces(board, true) + NumPieces(board, false) <= 10;
	}
}
