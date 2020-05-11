using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }
        public int Width { get; set; }

        public int Height { get; set; }  
        public int NumMines { get; set; }
        public Cell[,] Cells { get; set; }

        public Board(Minesweeper minesweeper, int width, int height, int mines)
        {
            this.Minesweeper = minesweeper;
            this.Width = width;
            this.Height = height;
            this.NumMines = mines;
            this.Cells = new Cell[width, height];
        }

        public void SetupBoard()
        {
            for (var i = 1; i <= Width; i++)
            {
                for (var i1 = 1; i1 <= Height; i1++)
                {
                    var c = new Cell
                    {
                        XLoc = i - 1,
                        YLoc = i1 - 1,
                        CellState = 1,
                        CellType = 0,
                        CellSize = 50,
                        Board = this
                    };
                    c.SetupDesign();

                    c.MouseDown += Cell_MouseClick;

                    this.Cells[i-1, i1-1] = c;

                    this.Minesweeper.Controls.Add(c);          
                }
            }
        }


        public void PlaceMines()
        {
            var minesPlaced = 0;
            var random = new Random();

            while (minesPlaced < this.NumMines)
            {
                int x = random.Next(0, this.Width);
                int y = random.Next(0, this.Height);

                if (this.Cells[x, y].SetMine())
                {
                    minesPlaced += 1;
                }
            }

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = this.Cells[x, y];
                    c.UpdateDisplay();
                    c.NumMines = c.GetNeighborMines().Count();
                }
            }
        }

        private void Cell_MouseClick(object sender, MouseEventArgs e)
        {
            var cell = (Cell) sender;

            if (cell.CellState == 0)
                return;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    cell.OnClick();
                    break;

                case MouseButtons.Right:
                    cell.OnFlag();
                    break;

                default:
                    return;
            }

            CheckForWin();
        }

        private void CheckForWin()
        {
            var correctMines = 0;
            var incorrectMines = 0;

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = this.Cells[x, y];
                    if (c.CellType == 1)
                        incorrectMines += 1;
                    if (c.CellType == 3)
                        correctMines += 1;
                }
            }

            if (correctMines == NumMines && incorrectMines == 0)
            {
                MessageBox.Show("Congratulations! You won.");
                RestartGame();
            }
        }

        public void RestartGame()
        {
            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var c = this.Cells[x, y];
                    this.Minesweeper.Controls.Remove(c);
                }
            }

            this.SetupBoard();
            this.PlaceMines();
        }
    }
}
