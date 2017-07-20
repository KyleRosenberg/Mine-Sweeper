using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        int numBombs;
        bool hasClickedTile = false;
        int safeTilesClicked = 0;
        int totalSafeTiles;
        int playTimeCentiseconds = 0;
        int bombsLeft;

        int holdTime = 0;

        PictureBox[][] tiles;

        PictureBox currentHold;

        bool gameOver = false;

        public Form1()
        {
            InitializeComponent();
            beginnerToolStripMenuItem_Click(this, new EventArgs());
        }

        private void beginnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame(9, 9, 10);
        }

        private void intermediateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame(16, 16, 40);
        }

        private void expertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame(30, 16, 99);
        }

        void newGame(int width, int height, int bombs)
        {
            hasClickedTile = false;
            panel1.Controls.Clear();
            numBombs = bombs;
            bombsLeft = bombs;
            tiles = new PictureBox[width][];
            for (int i = 0; i < width; i++)
            {
                tiles[i] = new PictureBox[height];
                for (int j = 0; j < height; j++)
                {
                    tiles[i][j] = new PictureBox();
                    panel1.Controls.Add(tiles[i][j]);
                    tiles[i][j].BackColor = Color.White;
                    tiles[i][j].Location = new Point(i * 50, j * 50);
                    tiles[i][j].Size = new Size(50, 50);
                    tiles[i][j].BorderStyle = BorderStyle.FixedSingle;
                    tiles[i][j].MouseDown += tileDown;
                    tiles[i][j].MouseUp += tileUp;
                    tiles[i][j].Tag = new Cell(false);
                }
            }
            totalSafeTiles = width * height - bombs;
            playTimeCentiseconds = 0;
            safeTilesClicked = 0;
            bombsDisplay.Text = "Bombs: " + bombsLeft;
            timeDisplay.Text = ((double)playTimeCentiseconds) / 100 + "";
            gameOver = false;
        }

        void setBombs(int x, int y)
        {
            Random r = new Random();
            int bombsMade = 0;
            List<Point> points = new List<Point>();
            while (bombsMade < numBombs)
            {
                int nextW = r.Next(0, tiles.Length), nextH = r.Next(0, tiles[0].Length);
                while (Math.Abs(nextW - x) < 2 || Math.Abs(nextH - y) < 2 || contains(points, new Point(nextW, nextH)))
                {
                    nextW = r.Next(0, tiles.Length);
                    nextH = r.Next(0, tiles[0].Length);
                }
                tiles[nextW][nextH].Tag = new Cell(true);
                //tiles[nextW][nextH].BackgroundImage = Properties.Resources.Bomb;
                tiles[nextW][nextH].BackgroundImageLayout = ImageLayout.Stretch;
                points.Add(new Point(nextW, nextH));
                bombsMade++;
                Console.WriteLine("Bomb made at " + nextW + ", " + nextH);
            }
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    if (tiles[i][j].Tag is Cell)
                    {
                        Cell c = tiles[i][j].Tag as Cell;
                        if (!c.IsBomb)
                        {
                            tiles[i][j].Tag = new Cell(false);
                        }
                    }
                    else
                    {
                        tiles[i][j].Tag = new Cell(false);
                    }
                }
            }
            setNumbers();
        }

        bool contains(List<Point> ps, Point p)
        {
            for (int i = 0; i < ps.Count; i++)
            {
                if (ps[i].X == p.X && ps[i].Y == p.Y) 
                {
                    Console.WriteLine("Duplicate found at " + p.X + ", " + p.Y);
                    return true;
                }
            }
            return false;
        }

        void setNumbers()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    Cell cell = tiles[i][j].Tag as Cell;
                    if (!cell.IsBomb)
                    {
                        cell.Number = getAdjacentBombs(tiles, new int[] { i, j });
                    }
                }
            }
        }

        private void tileDown(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                timer1.Enabled = true;
                holdTime = 0;
                currentHold = sender as PictureBox;
            }
        }

        private void tileUp(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (holdTime < 20&&!gameOver)
            {
                tileClick(sender);
            }
            currentHold = null;
        }
       
        private void tileClick(object sender)
        {
            PictureBox pb = sender as PictureBox;
            if (!hasClickedTile)
            {
                hasClickedTile = true;
                int[] temp = indexOf(tiles, pb);
                setBombs(temp[0], temp[1]);
                playTimer.Enabled = true;
            }
            Cell c = pb.Tag as Cell;
            if (!c.HasClicked && !c.HasFlag)
            {
                int[] temp = indexOf(tiles, pb);
                Console.WriteLine("Clicking at " + temp[0] + ", " + temp[1]);
                c.HasClicked = true;
                if (c.IsBomb)
                {
                    loseGame();
                }
                else
                {
                    pb.BackColor = Color.DarkGray;
                    safeTilesClicked++;
                    if (c.Number == 0)
                    {
                        clickAdjacentTiles(indexOf(tiles, pb));
                    }
                    else
                    {
                        pb.BackgroundImageLayout = ImageLayout.Stretch;
                        switch (c.Number)
                        {
                            case 6:
                                pb.BackgroundImage = Properties.Resources.Six;
                                break;
                            case 5:
                                pb.BackgroundImage = Properties.Resources.Five;
                                break;
                            case 4:
                                pb.BackgroundImage = Properties.Resources.Four;
                                break;
                            case 3:
                                pb.BackgroundImage = Properties.Resources.Three;
                                break;
                            case 2:
                                pb.BackgroundImage = Properties.Resources.Two;
                                break;
                            case 1:
                                pb.BackgroundImage = Properties.Resources.One;
                                break;
                        }
                    }
                    if (safeTilesClicked == totalSafeTiles)
                    {
                        winGame();
                    }
                }
            }
            else if (c.HasClicked)
            {
                int[] temp = indexOf(tiles, pb);
                if (getAdjacentFlags(tiles, temp) == c.Number)
                {
                    clickAdjacentTilesSafely(temp);
                }
            }
        }

        private void tileHold(object sender)
        {
            PictureBox pb = sender as PictureBox;
            Cell c = pb.Tag as Cell;
            if (!c.HasClicked)
            {
                if (c.HasFlag)
                {
                    c.HasFlag = false;
                    pb.BackgroundImage = null;
                    bombsLeft++;
                    bombsDisplay.Text = "Bombs: " + bombsLeft;
                }
                else
                {
                    c.HasFlag = true;
                    pb.BackgroundImage = Properties.Resources.Flag;
                    pb.BackgroundImageLayout = ImageLayout.Stretch;
                    bombsLeft--;
                    bombsDisplay.Text = "Bombs: " + bombsLeft;
                }
                pb.BackColor = Color.White;
            }
        }

        int[] indexOf(PictureBox[][] arr, PictureBox pb)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j<arr[i].Length; j++)
                {
                    if (arr[i][j].Equals(pb)) return new int[] { i, j };
                }
            }
            return new int[] { -1, -1 };
        }

        int getAdjacentBombs(PictureBox[][] arr, int[] loc)
        {
            int tot = 0;
            for (int i = Math.Max(0, loc[0] - 1); i < Math.Min(arr.Length, loc[0] + 2); i++)
            {
                for (int j = Math.Max(0, loc[1] - 1); j < Math.Min(arr[i].Length, loc[1] + 2); j++)
                {
                    Cell c = arr[i][j].Tag as Cell;
                    if (c.IsBomb) tot++;
                }
            }
            return tot;
        }

        int getAdjacentFlags(PictureBox[][] arr, int[] loc)
        {
            int tot = 0;
            for (int i = Math.Max(0, loc[0] - 1); i < Math.Min(arr.Length, loc[0] + 2); i++)
            {
                for (int j = Math.Max(0, loc[1] - 1); j < Math.Min(arr[i].Length, loc[1] + 2); j++)
                {
                    Cell c = arr[i][j].Tag as Cell;
                    if (c.HasFlag) tot++;
                }
            }
            return tot;
        }

        bool hasVisibleAdjacentTile(PictureBox[][] arr, int[] loc)
        {
            for (int i = Math.Max(0, loc[0] - 1); i < Math.Min(arr.Length, loc[0] + 2); i++)
            {
                for (int j = Math.Max(0, loc[1] - 1); j < Math.Min(arr[i].Length, loc[1] + 2); j++)
                {
                    Cell c = arr[i][j].Tag as Cell;
                    if (c.HasClicked&&(j!=0&&i!=0)) return true;
                }
            }
            return false;
        }

        void clickAdjacentTiles(int[] loc)
        {
            for (int i = Math.Max(0, loc[0] - 1); i < Math.Min(tiles.Length, loc[0] + 2); i++)
            {
                for (int j = Math.Max(0, loc[1] - 1); j < Math.Min(tiles[i].Length, loc[1] + 2); j++)
                {
                    if (i == loc[0] && j == loc[0]) { }
                    else
                    {
                        tileClick(tiles[i][j]);
                    }
                }
            }
        }

        void clickAdjacentTilesSafely(int[] loc)
        {
            for (int i = Math.Max(0, loc[0] - 1); i < Math.Min(tiles.Length, loc[0] + 2); i++)
            {
                for (int j = Math.Max(0, loc[1] - 1); j < Math.Min(tiles[i].Length, loc[1] + 2); j++)
                {
                    if (i == loc[0] && j == loc[0]) { }
                    else
                    {
                        Cell c = tiles[i][j].Tag as Cell;
                        if (!c.HasFlag&&!c.HasClicked)
                        {
                            tileClick(tiles[i][j]);
                        }
                    }
                }
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame(tiles.Length, tiles[0].Length, numBombs);
        }

        void winGame()
        {
            gameOver = true;
            playTimer.Enabled = false;
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    Cell c = tiles[i][j].Tag as Cell;
                    if (c.IsBomb)
                    {
                        tiles[i][j].BackgroundImage = Properties.Resources.Bomb;
                        tiles[i][j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }
            }
            MessageBox.Show(this, "Your time: " + ((double)playTimeCentiseconds)/100, "You Win!");
        }

        void loseGame()
        {
            gameOver = true;
            playTimer.Enabled = false;
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[i].Length; j++)
                {
                    Cell c = tiles[i][j].Tag as Cell;
                    if (c.HasFlag && !c.IsBomb)
                    {
                        tiles[i][j].BackgroundImage = Properties.Resources.NotBomb;
                        tiles[i][j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else if (c.IsBomb)
                    {
                        tiles[i][j].BackgroundImage = Properties.Resources.Bomb;
                        tiles[i][j].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                }
            }
            MessageBox.Show("You lose");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            holdTime++;
            Console.WriteLine(holdTime);
            if (holdTime > 30)
            {
                tileHold(currentHold);
                timer1.Enabled = false;
            }
        }

        private void playTimer_Tick(object sender, EventArgs e)
        {
            playTimeCentiseconds++;
            timeDisplay.Text = ((double)playTimeCentiseconds) / 100 + "";
        }

        private void cheatyHintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int[] temp = getRandomAdjacentBomb();
            if (temp[0] != -1)
            {
                PictureBox pb = tiles[temp[0]][temp[1]];
                Cell c = pb.Tag as Cell;
                pb.BackColor = Color.Red;
                MessageBox.Show(this, "If I were a cheaty robot who \nknew the answer, I would \nsay the red tile was a bomb.", "Dirty Cheater...");
            }
            else
            {
                MessageBox.Show(this, "Hmm I can't give you any cheaty hints in this scenerio...", "Dirty Cheater...");
            }
        }

        int[] getRandomAdjacentBomb()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int j = 0; j < tiles[0].Length; j++)
                {
                    if (hasVisibleAdjacentTile(tiles, new int[] { i, j }))
                    {
                        Cell c = tiles[i][j].Tag as Cell;
                        if (!c.HasFlag && !c.HasClicked&&c.IsBomb)
                        {
                            return new int[] { i, j };
                        }
                    }
                }
            }
            return new int[] { -1, -1 };
        }
    
    }
}
