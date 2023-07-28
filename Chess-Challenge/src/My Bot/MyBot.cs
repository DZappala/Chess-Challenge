using System;
using System.Collections.Generic;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    public NeuralNetwork nn = new(64, 64, 64);
    public Move Think(Board board, Timer timer)
    {
        var legalMoves = board.GetLegalMoves();
        var bestMove = legalMoves.First();
        var bestScore = float.MinValue;
        foreach (var move in legalMoves)
        {
            var moveScore = EvaluateMove(nn, board, move);
            if (moveScore > bestScore)
            {
                bestMove = move;
                bestScore = moveScore;
            }
        }
        return bestMove;
    }

    private static float EvaluateMove(NeuralNetwork nn, Board board, Move move)
    {
        // This function should convert the board state after the move to a suitable input for the neural network
        board.MakeMove(move); //make move is a void function
        var nnInput = ConvertBoardToNNInput(board);
        var nnOutput = nn.Forward(nnInput);
        board.UndoMove(move); //undo move is a void function

        return nnOutput.Max();
    }

    private static float[] ConvertBoardToNNInput(Board board)
    {
        var nnInput = new float[12 * 64];
        foreach (PieceType pieceType in Enum.GetValues<PieceType>())
        {
            if (pieceType == PieceType.None) continue;
            for (int color = 0; color < 2; color++)
            {
                ulong bitboard = GetBitboardForPieceType(board, pieceType, color == 0);
                for (int square = 0; square < 64; square++)
                {
                    nnInput[color * 6 * 64 + ((int)pieceType - 1) * 64 + square] = (bitboard & 1) == 1 ? 1 : 0;
                    bitboard >>= 1;
                }
            }
        }
        return nnInput;
    }

    private static ulong GetBitboardForPieceType(Board board, PieceType type, bool isWhite)
    {
        return board.GetPieceBitboard(type, isWhite);
    }

}

public class NeuralNetwork
{
    private readonly float[][] weights1;
    private readonly float[][] weights2;
    private readonly int inputCount;
    private readonly int hiddenCount;
    private readonly int outputCount;

    public NeuralNetwork(int inputCount, int hiddenCount, int outputCount)
    {
        this.inputCount = inputCount;
        this.hiddenCount = hiddenCount;
        this.outputCount = outputCount;

        weights1 = new float[this.inputCount][];
        for (int i = 0; i < this.inputCount; i++)
        {
            weights1[i] = new float[this.hiddenCount];
        }

        weights2 = new float[this.hiddenCount][];
        for (int i = 0; i < this.hiddenCount; i++)
        {
            weights2[i] = new float[this.outputCount];
        }

        Random rnd = new();
        for (int i = 0; i < this.inputCount; i++)
        {
            for (int j = 0; j < this.hiddenCount; j++)
            {
                weights1[i][j] = (float)rnd.NextDouble() * 2f - 1f; // Random float between -1 and 1
            }
        }

        for (int i = 0; i < this.hiddenCount; i++)
        {
            for (int j = 0; j < this.outputCount; j++)
            {
                weights2[i][j] = (float)rnd.NextDouble() * 2f - 1f; // Random float between -1 and 1
            }
        }
    }

    public float[] Forward(float[] inputs)
    {
        float[] hidden = new float[hiddenCount];
        for (int i = 0; i < inputCount; i++)
        {
            for (int j = 0; j < hiddenCount; j++)
            {
                hidden[j] += inputs[i] * weights1[i][j];
            }
        }

        for (int i = 0; i < hiddenCount; i++)
        {
            hidden[i] = Sigmoid(hidden[i]);
        }

        float[] output = new float[outputCount];
        for (int i = 0; i < hiddenCount; i++)
        {
            for (int j = 0; j < outputCount; j++)
            {
                output[j] += hidden[i] * weights2[i][j];
            }
        }

        return SoftMax(output);
    }

    private static float Sigmoid(float x)
    {
        return 1 / (1 + MathF.Exp(-x));
    }

    private static float[] SoftMax(float[] output)
    {
        float sum = output.Sum(x => MathF.Exp(x));
        return output.Select(x => MathF.Exp(x) / sum).ToArray();
    }

}