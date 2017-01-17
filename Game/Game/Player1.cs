using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class State
    {
  
        public Board Board;
        public bool? IsWin = null;   
        public Tuple<int, int> SelectedSquere;
        public bool myTurn;
        public List<State> childrenStates = new List<State>();
        public bool isExplored = false;


        public State(Board board, Tuple<int, int> SelectedSquere, bool turn)
        {
            this.Board = board;
            this.SelectedSquere = SelectedSquere;
            //IsWin = WinMove(board);
            myTurn = turn;
        }

        public void printState()
        {
            if (SelectedSquere != null)
            {
            Console.WriteLine(SelectedSquere.Item1.ToString()+","+SelectedSquere.Item2.ToString());
                Console.WriteLine(myTurn.ToString());    
            }
            for (int i = 0; i < Board._cols; i++)
            {
                for (int j = 0; j < Board._rows; j++)
                {
                    if (Board._board[j, i] == ' ')
                        Console.Write('A');
                    else
                        Console.Write(Board._board[j, i]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("-----------------------");
        }

        public void WinMove()
        {
            if (this.isExplored && this.childrenStates.Count == 0)
            {
                if (!myTurn)
                {
                    this.IsWin = false;

                }
                else
                {
                    this.IsWin = true;
                }
                return;
            }
            else
            {
                if (myTurn)
                {
                    this.IsWin = CheckForWinSituations();

                }
                else
                {
                    IsWin = !CheckForWinSituations();
                }
            }
        }

        public bool? CheckForWinSituations()
        {
            if ((Board._board[0,1]==' ' || Board._board[1, 0] == ' ') && Board._squaresLeft > 1)
            {
                return true;
            }
            int i = 0;
            while (i < Board._rows  && i < Board._cols  && Board._board[i,i] == 'X')
            {
                i++;
            }
            i--;
            if (Board._squaresLeft == i * i)
                return true;
            //if (i>=1 && Board._board[i-1,i] == 'X' && Board._board[i , i-1] == 'X')
            //{
            //    return true;
            //}
            
            
            return null;
        }
        public void calculateWinOrLose()
        {
            int Count = 0;
            WinMove();
            if (myTurn)
            {

                foreach (State child in childrenStates)
                {
                    if (child.IsWin == true)
                    {
                        IsWin = true;
                        break;
                    }
                    else if (child.IsWin == false)
                    {
                        Count++;
                    }
                }
                if (Count == childrenStates.Count && childrenStates.Count!=0)
                    IsWin = false;
            }
            else  //if its the op turn
            {
                foreach (State child in childrenStates)
                {
                    if (child.IsWin == false)
                    {
                        IsWin = false;
                        break;
                    }
                    else if (child.IsWin == true)
                    {
                        Count++;
                    }
                }
                if (Count == childrenStates.Count && childrenStates.Count != 0)
                    IsWin = true;
            }
        }
        public List<State> AllNextStates(double timesup, Stopwatch timer)
        {
            bool stop = false;
            List<State> nextMoves = new List<State>();
            State tmpBoard = new State(new Board(Board), SelectedSquere, myTurn);
            for (int i = 0; i < Board._cols && !stop; i++)
            {
                for (int j = 0; j < Board._rows && !stop; j++)
                {
                    if (timer.Elapsed.TotalMilliseconds > timesup)
                    {
                        stop = true;
                        break;
                    }
                    if (tmpBoard.Board.fillPlayerMove(j, i))
                    {
                        if (SelectedSquere == null)
                        {
                            nextMoves.Add(new State(tmpBoard.Board, new Tuple<int, int>(j, i), !myTurn));
                        }
                        else
                        {
                            nextMoves.Add(new State(tmpBoard.Board, tmpBoard.SelectedSquere, !myTurn));
                        }
                        tmpBoard = new State(new Board(Board), SelectedSquere, myTurn);
                    }
                }
            }
            if (!stop)
            {
                this.isExplored = true;
            }
            return nextMoves;
        }
    }




    /* -----------------------------------------------------------------------------------------------------------*/
    public class Player1
    {
        private Stopwatch timer;
        private TimeSpan myTime;
        private List<State> MovesList;
        private int timeInMilliseconds;
        public void getPlayers(ref string player1_1, ref string player1_2)  //fill players ids
        {
            player1_1 = "123456789";  //id1
            player1_2 = "123456789";  //id2
        }
        public Tuple<int, int> playYourTurn
        (
            Board board,
            TimeSpan timesup
        )

        {
            timeInMilliseconds = timesup.Milliseconds;
            timer = Stopwatch.StartNew();
            MovesList = new List<State>();
            Tuple<int, int> toReturn = null;
            State initState = new State(board, null, true);
            //timesup = new TimeSpan(0, 0, 0, 0, 50);
            buildMinMax(initState, timesup);
            solveMinMax();
            foreach (State move in initState.childrenStates)
            {
                if (move.IsWin == true)
                    toReturn = move.SelectedSquere;
            }
            if (toReturn == null)
            {
                toReturn = naiveMove(board);
            }
            //Console.WriteLine(toReturn.ToString());
            return toReturn;
        }

        private Tuple<int, int> naiveMove(Board board)
        {
            //board.printTheBoard();
            if (board._board[0, 1] == ' '  && board._squaresLeft > 1)
            {
                return new Tuple<int, int>(1,0);
            }
            if (board._board[1, 0] == ' ' && board._squaresLeft > 1)
            {
                return new Tuple<int, int>(0,1);
            }
            //even corner
            if (board._board[1, 1] == ' ' && board._squaresLeft % 2 == 0)
            {
                if (board._board[0, 2] == ' ')
                {
                    return new Tuple<int, int>(2, 0);
                }
                else if (board._board[2, 0] == ' ')
                {
                    return new Tuple<int, int>(0, 2);
                }
                else
                if (board._board[0, 3] == 'X' )
                {
                    int j = 3;
                    while (j < board._cols && board._board[0, j] == 'X') { j++;}
                    j--;
                    return new Tuple<int, int>(0,j);
                }
                else
                {
                    int j = 3;
                    while (j < board._rows && board._board[j, 0] == 'X') { j++; }
                    j--;
                    return new Tuple<int, int>(j, 0);
                }
                
            }
            //odd corner
            else if (board._board[1, 1] == ' ' && board._squaresLeft % 2 == 1)
            {
                if (board._board[0, 2] == ' ')
                {
                    return new Tuple<int, int>(2,0);
                }
                else if(board._board[2, 0] == ' ')
                {
                    return new Tuple<int, int>(0,2);
                }

                if (board._board[0, 3] == 'X')
                {
                    //board.printTheBoard();
                    int j = 3;
                    while (j < board._cols && board._board[0, j] == 'X') { j++; }
                    j--;
                    return new Tuple<int, int>(0, j-1);
                }
                else
                {
                    int j = 3;
                    while (j < board._rows && board._board[j, 0] == 'X') { j++; }
                    j--;
                    return new Tuple<int, int>(j - 1, 0);
                }
            }
            
            //prefect squere?
            Tuple<int,int> toReturnTuple = new Tuple<int, int>(-1,-1);
            int i = 0;
            while (i < board._rows && i < board._cols && board._board[i, i] == 'X')
            {
                i++;
            }
            i--;
            if (i +1< board._rows  && board._board[i+1, i] == 'X')
            {
                int j = i+1;
                while (j < board._rows  && board._board[j, i] == 'X')
                {
                    j++;
                }
                j--;
                toReturnTuple= new Tuple<int, int>(j, i);
            }
            else
            if ( i+1 < board._cols && board._board[i, i+1] == 'X')
            {
                int j = i + 1;
                while ( j < board._cols && board._board[i, j] == 'X')
                {
                    j++;
                }
                j--;
                toReturnTuple = new Tuple<int, int>(i, j);
            }
            else if (!(board._squaresLeft == i*i))
            {
                toReturnTuple = new Tuple<int, int>(i,i);
            }
            else
            {
                toReturnTuple = new Tuple<int, int>(1,1);
            }
            return toReturnTuple;
        }

        public void buildMinMax(State initState, TimeSpan timesup)
        {
            MovesList.Add(initState);
            for (int j = 0; j < MovesList.Count && timer.Elapsed.TotalMilliseconds < timesup.TotalMilliseconds*0.5; j++)
            {
                //MovesList[j].WinMove();
                if (MovesList[j].IsWin != true)
                {
                    MovesList[j].childrenStates = MovesList[j].AllNextStates(timesup.TotalMilliseconds*0.5,timer);
                    MovesList.AddRange(MovesList[j].childrenStates);
                }
            }
        }
        public void solveMinMax()
        {
            for (int i = MovesList.Count - 1; i >= 0; i--)
            {
                MovesList[i].calculateWinOrLose();
            }
        }

    }

}
