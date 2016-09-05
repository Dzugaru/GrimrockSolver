using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrimrockSolver
{
    class PortalPuzzleState : IState
    {
        public static readonly int[,] Moves = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        const int FieldSize = 3;
        const int DoorButtonX = 0, DoorButtonY = 2;
        const int ExitX = 2, ExitY = 0;

        public readonly int X, Y;
        public readonly int[,] Gates;
        public readonly bool DoorOpen;

        public readonly List<IAction> PastMoves;

        public bool Terminal { get { return X == ExitX && Y == ExitY && DoorOpen; } }

        public PortalPuzzleState(int x, int y, int[,] g, bool doorOpen, List<IAction> pastMoves)
        {
            X = x;
            Y = y;
            Gates = g;
            DoorOpen = doorOpen;
            PastMoves = pastMoves;
        }

        static bool InsideField(int x, int y)
        {
            return x >= 0 && y >= 0 && x < FieldSize && y < FieldSize;
        }

        public IState Move(IAction action)
        {
            MoveAction a = (MoveAction)action;
            int dx = a.Dx;
            int dy = a.Dy;

            int nx = X + dx;
            int ny = Y + dy;
            if (!InsideField(nx, ny)) return null;
            if (Gates[ny, nx] == 1) return null;

            int[,] nGates = new int[FieldSize, FieldSize];
            Array.Copy(Gates, nGates, FieldSize * FieldSize);
            for (int i = 0; i < 4; i++)
            {
                int gx = nx + Moves[i, 0];
                int gy = ny + Moves[i, 1];
                if (!InsideField(gx, gy)) continue;
                nGates[gy, gx] = nGates[gy, gx] == 1 ? 0 : 1;
            }

            bool doorOpen = this.DoorOpen || (nx == DoorButtonX && ny == DoorButtonY);
            List<IAction> pastMoves = new List<IAction>(this.PastMoves);
            pastMoves.Add(new MoveAction(dx, dy));

            PortalPuzzleState ns = new PortalPuzzleState(nx, ny, nGates, doorOpen, pastMoves);
            return ns;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(DoorOpen ? "op" : "cl");
            for (int i = 0; i < FieldSize; i++)
            {
                for (int j = 0; j < FieldSize; j++)
                {
                    if (i == Y && j == X) sb.Append("x");
                    else if (Gates[i, j] == 1) sb.Append("o");
                    else sb.Append(".");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public bool Equals(IState other)
        {
            PortalPuzzleState tOther = other as PortalPuzzleState;
            if (tOther == null) throw new InvalidOperationException();

            for (int i = 0; i < FieldSize; i++)
                for (int j = 0; j < FieldSize; j++)
                    if (Gates[i, j] != tOther.Gates[i, j])
                        return false;

            return X == tOther.X && Y == tOther.Y && DoorOpen == tOther.DoorOpen;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < FieldSize; i++)
                for (int j = 0; j < FieldSize; j++)
                    hash += Gates[i, j] << (i * FieldSize + j);

            hash += (DoorOpen ? 1 : 0) << (FieldSize * FieldSize);
            hash += (Y * FieldSize + X) << (FieldSize * FieldSize + 1);

            return hash;
        }

        public IEnumerable<IAction> Actions
        {
            get
            {
                for (int i = 0; i < 4; i++)
                {
                    yield return new MoveAction(Moves[i, 0], Moves[i, 1]);
                }
            }
        }

        public IEnumerable<IAction> ActionHistory
        {
            get
            {
                return PastMoves;
            }
        }
    }
}
