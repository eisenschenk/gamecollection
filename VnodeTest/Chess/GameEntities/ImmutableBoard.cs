using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Chess.GameEntities
{
    public class ImmutableBoard : IEnumerable<Piece>
    {
        private readonly Piece[] Board;


        public Piece this[(int X, int Y) position]
        {
            get => Board[position.Y * 8 + position.X];
        }

        public ImmutableBoard(Piece[] pieces)
        {
            Board = pieces;
        }

        public ImmutableBoard Move(Piece start, (int X, int Y) target)
        {
            Piece[] pieces = new Piece[64];
            Board.CopyTo(pieces, 0);

            pieces[To1D(target)] = start.Move(target);
            pieces[To1D(start.Position)] = null;

            return new ImmutableBoard(pieces);
        }

        public ImmutableBoard DeletePiece((int X, int Y) target)
        {
            Piece[] pieces = new Piece[64];
            Board.CopyTo(pieces, 0);

            pieces[To1D(target)] = null;

            return new ImmutableBoard(pieces);
        }

        public ImmutableBoard ReplacePiece((int X, int Y) target, Piece replacement)
        {
            Piece[] pieces = new Piece[64];
            Board.CopyTo(pieces, 0);

            pieces[To1D(target)] = replacement;

            return new ImmutableBoard(pieces);
        }

        private (int X, int Y) To2D(int index)
        {
            return (index % 8, index / 8);
        }
        private int To1D((int X, int Y) position)
        {
            return (position.X + position.Y * 8);
        }

        public IEnumerator<Piece> GetEnumerator()
        {
            return new BoardEnum(Board);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<Piece>)GetEnumerator();
        }

        public class BoardEnum : IEnumerator<Piece>
        {
            public Piece[] Pieces;
            int Position = -1;
            bool Disposed = false;
            SafeHandle Handle = new SafeFileHandle(IntPtr.Zero, true);

            public BoardEnum(Piece[] pieces)
            {
                Pieces = pieces;
            }
            public bool MoveNext()
            {
                Position++;
                return Position < Pieces.Length;
            }

            public void Reset()
            {
                Position = -1;
            }

            public void Dispose()
            {
                // Dispose of unmanaged resources.
                Dispose(true);
                // Suppress finalization.
                GC.SuppressFinalize(this);
            }
            protected virtual void Dispose(bool disposing)
            {
                if (Disposed)
                    return;

                if (disposing)
                {
                    // Free any other managed objects here.
                    //
                }

                Disposed = true;
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public Piece Current
            {
                get
                {
                    try
                    {
                        return Pieces[Position];
                    }
                    catch
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }
    }

}
