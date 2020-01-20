﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess.GameEntities;
using static VnodeTest.Chess.Enums;

namespace VnodeTest.GameEntities
{
    class Knight : Piece
    {
        public Knight((int X, int Y) position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Knight;
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            var returnValues = new List<(int X, int Y)>(8);
            for (int index = -1; index < 2; index += 2)
            {
                //getting moves to the left and then to the right of the knight
                returnValues.Add((Position.X + index, Position.Y - 2));
                returnValues.Add((Position.X + index, Position.Y + 2));
                returnValues.Add((Position.X - 2, Position.Y + index));
                returnValues.Add((Position.X + 2, Position.Y + index));
            }
            return returnValues.Where(p => p.X >= 0 && p.X < 8 && p.Y >= 0 && p.Y < 8
            //@phil
            && (gameboard[p] != null == false || gameboard[p] != null == true && gameboard[p].Color != Color));
        }

        public override Piece Copy() => new Knight(Position, Color);
    }
}