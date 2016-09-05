using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrimrockSolver
{
    class HamletPuzzleState : IState
    {
        public static readonly int[,] Moves = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        const int FieldW = 5, FieldH = 4;
        const int ExitX = 3, ExitY = 0;
        static readonly List<(int x, int y)> LeverCoords = new List<(int x, int y)>()
        {
            (0,1), (0,2), (4,1), (4,2)
        };

        public readonly int X, Y;
        public readonly int[,] Platforms;
        public readonly bool[] Levers;

        public readonly List<IAction> PastMoves;

        public bool Terminal { get { return X == ExitX && Y == ExitY && Levers.All(b => b); } }

        public HamletPuzzleState(int x, int y, int[,] p, bool[] levers, List<IAction> pastMoves)
        {
            X = x;
            Y = y;
            Platforms = p;
            Levers = levers;
            PastMoves = pastMoves;
        }

        static bool InsideField(int x, int y)
        {
            return x >= 0 && y >= 0 && x < FieldW && y < FieldH;
        }

        public IState Move(IAction action)
        {
            MoveAction a = (MoveAction)action;
            int dx = a.Dx;
            int dy = a.Dy;

            int nx = X + dx;
            int ny = Y + dy;
            if (!InsideField(nx, ny)) return null;
            if (Platforms[ny, nx] == 0) return null;

            int[,] nPls = new int[FieldH, FieldW];
            Array.Copy(Platforms, nPls, FieldH * FieldW);
            for (int i = 0; i < 4; i++)
            {
                int gx = nx + Moves[i, 0];
                int gy = ny + Moves[i, 1];
                if (!InsideField(gx, gy)) continue;
                nPls[gy, gx] = nPls[gy, gx] == 1 ? 0 : 1;
            }

            bool[] nLevers = new bool[Levers.Length];
            Array.Copy(Levers, nLevers, Levers.Length);

            for (int i = 0; i < Levers.Length; i++)
            {
                (int lx, int ly) = LeverCoords[i];
                nLevers[i] = nLevers[i] || (nx == lx && ny == ly);
            }
            
            List<IAction> pastMoves = new List<IAction>(this.PastMoves);
            pastMoves.Add(new MoveAction(dx, dy));

            HamletPuzzleState ns = new HamletPuzzleState(nx, ny, nPls, nLevers, pastMoves);
            return ns;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new string(Levers.Select(b => b ? '1' : '0').ToArray()));
            for (int i = 0; i < FieldH; i++)
            {
                for (int j = 0; j < FieldW; j++)
                {
                    if (i == Y && j == X) sb.Append("x");
                    else if (Platforms[i, j] == 1) sb.Append("o");
                    else sb.Append(".");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public bool Equals(IState other)
        {
            HamletPuzzleState tOther = other as HamletPuzzleState;
            if (tOther == null) throw new InvalidOperationException();

            for (int i = 0; i < FieldH; i++)
                for (int j = 0; j < FieldW; j++)
                    if (Platforms[i, j] != tOther.Platforms[i, j])
                        return false;

            return X == tOther.X && Y == tOther.Y && Levers.SequenceEqual(tOther.Levers);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < FieldH; i++)
                for (int j = 0; j < FieldW; j++)
                    hash += Platforms[i, j] << (i * FieldW + j);

            for (int i = 0; i < Levers.Length; i++)
                hash += (Levers[i] ? 1 : 0) << (FieldW * FieldH + i);
            hash += (Y * FieldW + X) << (FieldW * FieldH + Levers.Length);

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
