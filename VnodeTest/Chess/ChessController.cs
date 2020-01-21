using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Chess
{
    public class ChessController
    {




        public static string GetSprite(PieceColor color, PieceValue value)
        {
            return value switch
            {
                PieceValue.King => color == PieceColor.White ? "\u2654" : "\u265A",
                PieceValue.Queen => color == PieceColor.White ? "\u2655" : "\u265B",
                PieceValue.Rook => color == PieceColor.White ? "\u2656" : "\u265C",
                PieceValue.Bishop => color == PieceColor.White ? "\u2657" : "\u265D",
                PieceValue.Knight => color == PieceColor.White ? "\u2658" : "\u265E",
                PieceValue.Pawn => color == PieceColor.White ? "\u2659" : "\u265F",
                _ => ""
            };
        }
    }
}
