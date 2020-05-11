using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Core
{
    public class GridDimensions
    {
        public int Rows { get; set; }
        public int Colums { get; set; }

        public GridDimensions()
        {
        }

        public GridDimensions(int rows, int colums)
        {
            Rows = rows;
            Colums = colums;
        }
    }
}
