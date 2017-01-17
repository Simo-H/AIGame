using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Board
    {
        public int      _rows;
        public int      _cols;
        public char[,]  _board;
        public int      _squaresLeft;
        public Board
        (
            int rows, 
            int cols
        )
        {
            _rows           = rows;
            _cols           = cols;
            _squaresLeft    = rows * cols;
            createBoard();
        }
        public Board
        (
            Board toCopy
        )
        {
            _rows           = toCopy._rows;
            _cols           = toCopy._cols;
            _squaresLeft    = toCopy._squaresLeft;
            copyBoard(toCopy._board);
        }
        private void createBoard()
        {
            _board  = new char[_rows, _cols];
            for(int i = 0; i < _rows; i++)
                for(int j = 0; j < _cols; j++)
                {
                    _board[i,j] = 'X';
                }
        }
        private void copyBoard
        (
            char[,] board
        )
        {
            _board = new char[_rows, _cols];
            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _cols; j++)
                {
                    _board[i, j] = board[i, j];
                }
        }
        public bool isTheGameEnded()
        {
            if (_squaresLeft == 0)
                return true;
            return false;
        }
       
        public bool fillPlayerMove
        (
            int row, 
            int col
        )
        {
            if (!isLegalMove(row, col))
                return false;
            deleteSquares(row, col);
            return true;
        }

        public bool isLegalMove
        (
            int row,
            int col
        )
        {
            if (row > _rows - 1 ||
                col > _cols - 1 ||
                row < 0         ||
                col < 0         ||
                _board[row, col] != 'X')
                return false;
            return true;
        }

        private void deleteSquares
        (
            int row,
            int col
        )
        {
            for (int i = row; i < _rows; i++)
                for (int j = col; j < _cols; j++)
                    if (_board[i, j] == 'X')
                    {
                        _board[i, j] = ' ';
                        _squaresLeft--;
                    }
        }

        public void printTheBoard()
        {
            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _cols; j++)
                { 
                    if(j == 0)
                        Console.Write("| ");
                    Console.Write(_board[i, j]);
                    Console.Write(" | ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
    }
}
