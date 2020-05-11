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
        public int XLoc { get; set; }
        public int YLoc { get; set; }     
        public int CellSize { get; set; }
        public int CellState { get; set; }
        public int CellType { get; set; }
        public int NumMines { get; set; }
        public Board Board { get; set; }

        public void SetupDesign()
        {
            this.Location = new Point(XLoc * CellSize, YLoc * CellSize);
            this.Size = new Size(CellSize, CellSize);
            this.UseVisualStyleBackColor = false;
            this.Font = new Font("Verdana", 15.75F, FontStyle.Bold);
            this.SetStyle(ControlStyles.Selectable, false);
        }

        public bool SetMine()
        {
            if (CellType != 1 && CellType != 3) CellType = 1;
            else return false;
            return true;
        }

        public void OnFlag()
        {
            switch (this.CellType)
            {
                case 0:
                    this.CellType = 2;
                    break;
                case 1:
                    this.CellType = 3;
                    break;
                case 2:
                    this.CellType = 0;
                    break;
                case 3:
                    this.CellType = 1;
                    break;
                default:
                    throw new Exception(string.Format("Unknown cell type {0}", this.CellType));
            }

            this.UpdateDisplay();
        }

        public void OnClick()
        {
            if (CellType == 3 || CellType == 2)
            {
                return;
            }
            if (CellState != 0)
            {
                CellState = 0;
                UpdateDisplay();
                if (CellType == 1)
                {
                    MessageBox.Show("Sorry, you lost!");
                    Board.RestartGame();
                    return;
                }
                if (GetNeighborMines().Count == 0)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int i1 = -1; i1 < 2; i1++)
                        {
                            if (i == 0 && i1 == 0) continue;
                            if (XLoc + i < 0 || XLoc + i == Board.Width) continue;
                            if (YLoc + i1 < 0 || YLoc + i1 == Board.Height) continue;
                            Board.Cells[XLoc + i, YLoc + i1].OnClick();
                        }
                    }
                }
            }
        }

        public List<Cell> GetNeighborMines()
        {
            var neighbors = new List<Cell>();

            for (var x = -1; x < 2; x++)
            {
                for (var y = -1; y < 2; y++)
                {
                    // Can't be your own neighbor!
                    if (x == 0 && y == 0)
                        continue;

                    // Cell would be out of bounds
                    if (XLoc + x < 0 || XLoc + x >= Board.Width || YLoc + y < 0 || YLoc + y >= Board.Height)
                        continue;

                    if (Board.Cells[XLoc + x, YLoc + y].CellType == 1)
                    {
                        neighbors.Add(Board.Cells[XLoc + x, YLoc + y]);
                    }
                }
            }

            return neighbors;
        }

        public void UpdateDisplay()
        {
            if (this.CellType == 2 ||
                this.CellType == 3)
            {
                this.BackColor = Color.Gray;
                this.ForeColor = Color.White;
                this.Text = "?";
                return;
            }

            if (this.CellState == 1)
            {
                this.BackColor = Color.Gray;
                this.ForeColor = Color.Gray;
                this.Text = string.Empty;
                return;
            }

            if (this.CellType == 1)
            {
                this.BackColor = Color.DarkRed;
                this.ForeColor = Color.White;
                this.Text = "M";
                return;
            }

            if (this.CellType == 0)
            {
                this.BackColor = Color.LightGray;
                this.ForeColor = this.GetCellColour();
                this.Text = this.NumMines > 0 ? string.Format("{0}", this.NumMines) : string.Empty;
            }
        }

        private Color GetCellColour()
        {
            switch (this.NumMines)
            {
                case 1:
                    return ColorTranslator.FromHtml("0x0000FE"); // 1
                case 2:
                    return ColorTranslator.FromHtml("0x186900"); // 2
                case 3:
                    return ColorTranslator.FromHtml("0xAE0107"); // 3
                case 4:
                    return ColorTranslator.FromHtml("0x000177"); // 4
                case 5:
                    return ColorTranslator.FromHtml("0x8D0107"); // 5
                case 6:
                    return ColorTranslator.FromHtml("0x007A7C"); // 6
                case 7:
                    return ColorTranslator.FromHtml("0x902E90"); // 7
                case 8:
                    return ColorTranslator.FromHtml("0x000000"); // 8
                default:
                    return ColorTranslator.FromHtml("0xffffff");
            }
        }
    }
}
