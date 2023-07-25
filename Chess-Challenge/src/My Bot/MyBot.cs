using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
	private readonly int _pawnValue;
	private readonly int _knightValue;
	private readonly int _bishopValue;
	private readonly int _rookValue;
	private readonly int _queenValue;
	private readonly int _kingValue;
	private readonly int _depth;
	private readonly ulong[] _bitboards;

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
}
