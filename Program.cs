using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrimrockSolver
{
    interface IAction
    {

    }

    interface IState : IEquatable<IState>
    {
        bool Terminal { get; }
        IState Move(IAction action);
        IEnumerable<IAction> Actions { get; }
        IEnumerable<IAction> ActionHistory { get; }
    }

    struct MoveAction : IAction
    {
        public int Dx, Dy;

        public MoveAction(int dx, int dy)
        {
            Dx = dx;
            Dy = dy;
        }
    }

    

    class Program
    {
        static void Main(string[] args)
        {
            //Puzzle1();            
            Puzzle2();
            Console.ReadLine();
        }

        static void Puzzle1()
        {
            PortalPuzzleState init = new PortalPuzzleState(1, 2, new int[3, 3]
                {
                    { 0, 1, 1 },
                    { 0, 0, 0 },
                    { 0, 0, 0 }
                }, false, new List<IAction>());

            IState solution = Solve(init);

            if (solution != null)
            {
                PrintMoves(solution);
            }
        }

        static void Puzzle2()
        {
            HamletPuzzleState init = new HamletPuzzleState(1, 3, new int[,]
                {
                    { 0, 0, 0, 0, 1 },
                    { 1, 0, 1, 0, 0 },
                    { 0, 1, 0, 0, 0 },
                    { 0, 1, 1, 0, 1 },
                }, new bool[4], new List<IAction>());

            IState solution = Solve(init);

            if (solution != null)
            {
                PrintMoves(solution);
            }
        }

        static void PrintMoves(IState solution)
        {
            foreach (MoveAction mv in solution.ActionHistory)
            {
                if (mv.Dx == -1 && mv.Dy == 0) Console.WriteLine("Left");
                else if (mv.Dx == 1 && mv.Dy == 0) Console.WriteLine("Right");
                else if (mv.Dx == 0 && mv.Dy == -1) Console.WriteLine("Up");
                else if (mv.Dx == 0 && mv.Dy == 1) Console.WriteLine("Down");
            }
        }

        static IState Solve(IState init)
        {
            HashSet<IState> visited = new HashSet<IState>();
            Queue<IState> front = new Queue<IState>();
            front.Enqueue(init);

            IState solution = null;
            do
            {
                IState c = front.Dequeue();

                foreach (IAction a in c.Actions)
                {
                    IState n = c.Move(a);
                    if (n != null && !visited.Contains(n))
                    {
                        if (n.Terminal)
                        {
                            solution = n;
                            break;
                        }

                        visited.Add(n);
                        front.Enqueue(n);

                        //Console.WriteLine(n);
                        //Console.ReadLine();
                    }
                }

                if (solution != null) break;

            } while (front.Count > 0);

            return solution;
        }
    }
}
