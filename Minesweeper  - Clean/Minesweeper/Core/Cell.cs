using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper.Core
{
    public class Cell : Button
    {
        public GridPosition GridPosition;
        public CellState CellState { get; set; }
        public CellType CellType { get; set; }
        public int NeighboringMines { get; set; }
        public Board Board { get; set; }

        public Cell(GridPosition gridPosition, Board board) 
        {
            GridPosition = gridPosition;
            CellState = CellState.Closed;
            CellType = CellType.Empty;
            NeighboringMines = 0;
            Board = board;
            SetupLookAndFeel();
            SetMouseDownEventHandler(CellMouseClick);
            UpdateDisplay();
        }

        internal void ResetState()
        {
            CellState = CellState.Closed;
            CellType = CellType.Empty;
            NeighboringMines = 0;

            //Ovo sam zaboravio u videu. Bitno je da bi ispravno funkcionisala aplikacija. Uvek kad uradite reset, da se broj neotvorenih celija vrati na pocetno stanje
            Board.UnopenedCells = Board.GridDimensions.Rows * Board.GridDimensions.Colums;
            UpdateDisplay();
        }

        public void SetupLookAndFeel()
        {
            Location = new Point(GridPosition.Row * Constants.CellSize, GridPosition.Column * Constants.CellSize);
            Size = new Size(Constants.CellSize, Constants.CellSize);
            UseVisualStyleBackColor = false;
            Font = new Font("Verdana", 15.75F, FontStyle.Bold);
            SetStyle(ControlStyles.Selectable, false);
        }

        public bool IsMine()
        {
            return CellType == CellType.Mine;
        }

        public bool IsIrrelevantCellTypeForOpening()
        {
            if (CellType == CellType.FlaggedMine || CellType == CellType.FlaggedEmpty
                || CellState == CellState.Opened) return true;
            return false;
        }

        internal void MakeMine()
        {
            CellType = CellType.Mine;
            UpdateNeighbourMineNumber();
        }

        public void SetMouseDownEventHandler(MouseEventHandler mouseEventHandler)
        {
            MouseDown += mouseEventHandler;
        }

        private void CellMouseClick(object sender, MouseEventArgs e)
        {
            Cell cell = (Cell)sender;

            if (cell.CellState == CellState.Opened) return;

            handleMouseEvents(cell, e);

            Board.CheckForWin();
        }

        public void handleMouseEvents(Cell cell, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left) cell.Open();
                else if (e.Button == MouseButtons.Right) cell.Flag();
            }
            catch (InvalidCellStateException)
            {
                MessageBox.Show("Sorry, this type does not exist!");
            }
        }

        public void Flag()
        {
            if (CellType == CellType.Mine) CellType = CellType.FlaggedMine;
            else if (CellType == CellType.Empty) CellType = CellType.FlaggedEmpty;
            else if (CellType == CellType.FlaggedEmpty) CellType = CellType.Empty;
            else if (CellType == CellType.FlaggedMine) CellType = CellType.Mine;
            else throw new InvalidCellStateException();

            UpdateDisplay();
        }

        public void Open()
        {
            if (IsIrrelevantCellTypeForOpening()) return;
            
            OpenCell();
            UpdateDisplay();

            if (IsMine())
            {
                Board.GameOver();
                return;
            }
            if (NeighboringMines == 0)
            {
                OpenNeighbouringCells();
            }
        }

        private void OpenNeighbouringCells()
        {
            for (int rowMove = -1; rowMove <= 1; rowMove++)
            {
                for (int columnMove = -1; columnMove <= 1; columnMove++)
                {
                    if (IsItSelf(rowMove, columnMove)) continue;
                    if (IsOutOfBounds(rowMove, columnMove)) continue;
                    Board.Cells[GridPosition.Row + rowMove, GridPosition.Column + columnMove].Open();
                }
            }
        }

        private void OpenCell()
        {
            CellState = CellState.Opened;
            Board.UnopenedCells--;
        }

        public bool IsItSelf(int rowMove, int columnMove)
        {
            return rowMove == 0 && columnMove == 0;
        }

        public bool IsOutOfBounds(int rowMove, int columnMove)
        {
            if (GridPosition.Row + rowMove < 0 || GridPosition.Row + rowMove == Board.GridDimensions.Rows) return true;
            if (GridPosition.Column + columnMove < 0 || GridPosition.Column + columnMove == Board.GridDimensions.Colums) return true;
            return false;
        }

        public void UpdateNeighbourMineNumber()
        {
            for (int rowMove = -1; rowMove <= 1; rowMove++)
            {
                for (int columnMove = -1; columnMove <= 1; columnMove++)
                {
                    if (IsItSelf(rowMove, columnMove)) continue;
                    if (IsOutOfBounds(rowMove, columnMove)) continue;
                    IncrementNeighbouringMines(rowMove, columnMove);
                }
            }
        }

        public void IncrementNeighbouringMines(int rowMove, int columnMove)
        {
            Board.Cells[GridPosition.Row + rowMove, GridPosition.Column + columnMove].NeighboringMines++;
        }

        public void UpdateDisplay()
        {
            if (CellType == CellType.FlaggedMine || CellType == CellType.FlaggedEmpty)
                ChangeCellVisualApperence(Color.Gray, Color.LightGray, Constants.Flagged);

            else if (CellState == CellState.Closed)
                ChangeCellVisualApperence(Color.Gray, Color.Gray, string.Empty);

            else if (CellType == CellType.Mine)
                ChangeCellVisualApperence(Color.DarkRed, Color.White, Constants.Mine);

            else if (this.CellType == CellType.Empty)
                ChangeCellVisualApperence(Color.LightGray, GetCellColour(), NeighboringMines > 0 ? NeighboringMines.ToString() : string.Empty);
        }

        private void ChangeCellVisualApperence(Color backColor, Color foreColor, string text)
        {
            BackColor = backColor;
            ForeColor = foreColor;
            Text = text;
        }

        private Color GetCellColour()
        {
            switch (NeighboringMines)
            {
                case 1: return GetColorForEmptyCells("0x0000FE");
                case 2: return GetColorForEmptyCells("0x186900");
                case 3: return GetColorForEmptyCells("0xAE0107");
                case 4: return GetColorForEmptyCells("0x000177");
                case 5: return GetColorForEmptyCells("0x8D0107");
                case 6: return GetColorForEmptyCells("0x007A7C");
                case 7: return GetColorForEmptyCells("0x902E90");
                case 8: return GetColorForEmptyCells("0x000000");
                default: return GetColorForEmptyCells("0xffffff");
            }
        }

        public Color GetColorForEmptyCells(string color)
        {
            return ColorTranslator.FromHtml(color);
        }
    }
}
