using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    public class Cell
    {
        bool isBomb;
        public Boolean IsBomb
        {
            get { return isBomb; }
            set { isBomb = value; }
        }

        int number;
        public Int32 Number
        {
            get { return number; }
            set { number = value; }
        }

        bool hasClicked;
        public bool HasClicked
        {
            get { return hasClicked; }
            set { hasClicked = value; }
        }

        bool hasFlag;
        public bool HasFlag
        {
            get { return hasFlag; }
            set { hasFlag = value; }
        }

        public Cell(bool isBomb)
        {
            this.isBomb = isBomb;
        }
    }
}
