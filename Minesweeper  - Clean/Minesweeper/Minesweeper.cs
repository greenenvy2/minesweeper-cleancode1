using Minesweeper.Core;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Minesweeper : Form
    {
        public Minesweeper()
        {
            InitializeComponent();

            Board board = new Board(this, new GridDimensions(9, 9), 10);
            board.SetupBoardCells();
            board.PlaceMines();
        }
    }
}
