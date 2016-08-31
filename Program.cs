using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrimrockSolver
{
    struct State : IEquatable<State>
    {
        public static readonly int[,] Moves = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        const int FieldSize = 3;
        const int DoorButtonX = 0, DoorButtonY = 2;
        const int ExitX = 2, ExitY = 0;        

        public readonly int X, Y;
        public readonly int[,] Gates;
        public readonly bool DoorOpen;

        public readonly List<(int,int)> PastMoves;

        public bool Terminal { get { return X == ExitX && Y == ExitY && DoorOpen; } }

        public State(int x, int y, int[,] g, bool doorOpen, List<(int,int)> pastMoves)
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

        public State? Move(int dx, int dy)
        {
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
            List<(int, int)> pastMoves = new List<(int, int)>(this.PastMoves);
            pastMoves.Add((dx, dy));

            State ns = new State(nx, ny, nGates, doorOpen, pastMoves);
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

        public bool Equals(State other)
        {   
            for (int i = 0; i < FieldSize; i++)
                for (int j = 0; j < FieldSize; j++)
                    if (Gates[i, j] != other.Gates[i, j])
                        return false;

            return X == other.X && Y == other.Y && DoorOpen == other.DoorOpen;
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            State init = new State(1, 2, new int[3, 3]
                {
                    { 0, 1, 1 },
                    { 0, 0, 0 },
                    { 0, 0, 0 }
                }, false, new List<(int, int)>());            

            HashSet<State> visited = new HashSet<State>();
            Queue<State> front = new Queue<State>();
            front.Enqueue(init);

            State? solution = null;
            do
            {
                State c = front.Dequeue();                

                for (int i = 0; i < 4; i++)
                {
                    int dx = State.Moves[i, 0];
                    int dy = State.Moves[i, 1];

                    State? n = c.Move(dx, dy);
                    if(n != null && !visited.Contains(n.Value))
                    {
                        if (n.Value.Terminal)
                        {
                            solution = n;
                            break;
                        }

                        visited.Add(n.Value);
                        front.Enqueue(n.Value);

                        //Console.WriteLine(n);
                        //Console.ReadLine();
                    }
                }

            } while (front.Count > 0);


            if(solution != null)
            {
                foreach(var mv in solution.Value.PastMoves)
                {
                    //Doesn't work for now...
                    //switch(mv)
                    //{
                    //    case (-1, 0): Console.WriteLine("Left"); break;
                    //    case (1, 0): Console.WriteLine("Right"); break;
                    //    case (0, -1): Console.WriteLine("Up"); break;
                    //    case (0, 1): Console.WriteLine("Down"); break;
                    //}  
                    
                    if(mv.Equals((-1, 0))) Console.WriteLine("Left");
                    else if (mv.Equals((1, 0))) Console.WriteLine("Right");
                    else if (mv.Equals((0, -1))) Console.WriteLine("Up");
                    else if (mv.Equals((0, 1))) Console.WriteLine("Down");
                }
            }
            
            Console.ReadLine();
        }
    }
}
