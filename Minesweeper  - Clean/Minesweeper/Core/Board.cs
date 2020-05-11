using System;
using System.Linq;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }
        public GridDimensions GridDimensions { get; set; }
        public int Mines { get; set; }
        public Cell[,] Cells { get; set; }
        public int UnopenedCells { get; set; }

        public Board(Minesweeper minesweeper, GridDimensions gridDimensions, int mines)
        {
            Minesweeper = minesweeper;
            GridDimensions = gridDimensions;
            Mines = mines;
            Cells = new Cell[gridDimensions.Rows, gridDimensions.Colums];
            UnopenedCells = gridDimensions.Rows * gridDimensions.Colums;
        }

        public void SetupBoardCells()
        {
            for (int row = 0; row < GridDimensions.Rows; row++)
            {
                for (int column = 0; column < GridDimensions.Colums; column++)
                {
                    Cell cell = new Cell(new GridPosition(row, column), this);                
                    Cells[row, column] = cell;
                    Minesweeper.Controls.Add(cell);          
                }
            }
        }

        public void PlaceMines()
        {
            int minesPlaced = 0;
            Random randomNumberGenerator = new Random();

            while (minesPlaced < Mines)
            {
                int row = randomNumberGenerator.Next(GridDimensions.Rows);
                int column = randomNumberGenerator.Next(GridDimensions.Colums);

                if (!Cells[row, column].IsMine())
                {
                    Cells[row, column].MakeMine();
                    minesPlaced += 1;
                }
            }
        }

        public void CheckForWin()
        {
            if (UnopenedCells == Mines)
            {
                MessageBox.Show("Congratulations! You won.");
                RestartGame();
            }
        }

        public void GameOver()
        {
            MessageBox.Show("Sorry, you lost!");
            RestartGame();
        }

        public void RestartGame()
        {
            ResetBoardCells();
            PlaceMines();
        }

        public void ResetBoardCells()
        {
            for (int row = 0; row < GridDimensions.Rows; row++)
            {
                for (int column = 0; column < GridDimensions.Colums; column++)
                {
                    Cells[row, column].ResetState();
                }
            }
        }
    }
}
