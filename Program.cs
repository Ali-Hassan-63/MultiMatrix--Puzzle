using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _15_Puzzle_Game_Tree
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string[] options = { "Easy (2x2)", "Medium (3x3)", "Hard (4x4)" };
            string level = "Hard";
            var levelForm = new LevelForm();
            levelForm.ShowDialog();
            level = levelForm.SelectedLevel;

            int size = 4;
            if (level == "Easy") size = 2;
            else if (level == "Medium") size = 3;

            Application.Run(new PuzzleForm(size, level));
        }
    }

    public class TreeNode
    {
        public int[,] State;
        public List<TreeNode> Children = new List<TreeNode>();
        public TreeNode Parent;

        public TreeNode(int[,] state, TreeNode parent = null)
        {
            State = Clone(state);
            Parent = parent;
        }

        private int[,] Clone(int[,] original)
        {
            int size = original.GetLength(0);
            int[,] copy = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    copy[i, j] = original[i, j];
            return copy;
        }
    }

    public class LevelForm : Form
    {
        public string SelectedLevel { get; private set; }

        public LevelForm()
        {
            this.Text = "Select Level";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterScreen;

            Button easy = new Button { Text = "Easy (2x2)", Location = new Point(50, 20), Size = new Size(100, 40) };
            Button medium = new Button { Text = "Medium (3x3)", Location = new Point(50, 70), Size = new Size(100, 40) };
            Button hard = new Button { Text = "Hard (4x4)", Location = new Point(160, 45), Size = new Size(100, 40) };

            easy.Click += (s, e) => { SelectedLevel = "Easy"; this.Close(); };
            medium.Click += (s, e) => { SelectedLevel = "Medium"; this.Close(); };
            hard.Click += (s, e) => { SelectedLevel = "Hard"; this.Close(); };

            Controls.Add(easy);
            Controls.Add(medium);
            Controls.Add(hard);
        }
    }

    public class NameInputForm : Form
    {
        public string PlayerName { get; private set; }

        public NameInputForm()
        {
            this.Text = "Enter Player Name";
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.CenterScreen;

            var label = new Label { Text = "Enter your name:", Location = new Point(10, 10) };
            var input = new TextBox { Location = new Point(10, 40), Width = 260 };
            var submit = new Button { Text = "Submit", Location = new Point(100, 80), Size = new Size(100, 30) };

            submit.Click += (s, e) => { PlayerName = input.Text; this.Close(); };

            Controls.Add(label);
            Controls.Add(input);
            Controls.Add(submit);
        }
    }

    public class PuzzleForm : Form
    {
        private Button[,] buttons;
        private int[,] board;
        private int boardSize;
        private int emptyRow, emptyCol;
        private Label moveLabel;
        private Button undoButton;
        private Button redoButton;
        private string playerName;
        private string level;
        private Stopwatch stopwatch = new Stopwatch();
        private int moveCount = 0;

        private TreeNode root;
        private TreeNode currentNode;

        public PuzzleForm(int size, string level)
        {
            this.boardSize = size;
            this.level = level;
            this.Text = $"Puzzle Game - {level}";
            this.Size = new Size(boardSize * 100 + 40, boardSize * 100 + 150);

            buttons = new Button[boardSize, boardSize];
            board = new int[boardSize, boardSize];
            playerName = GetPlayerName();

            InitializeBoard();
            ShuffleBoard();
            InitializeUI();
            UpdateUI();

            root = new TreeNode(board);
            currentNode = root;

            stopwatch.Start();
        }

        private string GetPlayerName()
        {
            using (var nameForm = new NameInputForm())
            {
                nameForm.ShowDialog();
                return nameForm.PlayerName;
            }
        }

        private void InitializeBoard()
        {
            int num = 1;
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    board[i, j] = num++;
            board[boardSize - 1, boardSize - 1] = 0;
            emptyRow = boardSize - 1;
            emptyCol = boardSize - 1;
        }

        private void ShuffleBoard()
        {
            Random rand = new Random();
            string[] directions = { "up", "down", "left", "right" };
            for (int i = 0; i < 200; i++)
                Move(directions[rand.Next(4)], false);
            moveCount = 0;
        }

        private void InitializeUI()
        {
            moveLabel = new Label
            {
                Text = "Moves: 0",
                Location = new Point(10, boardSize * 100 + 10),
                AutoSize = true
            };
            this.Controls.Add(moveLabel);

            undoButton = new Button
            {
                Text = "Undo",
                Location = new Point(120, boardSize * 100 + 10),
                Size = new Size(100, 30)
            };
            undoButton.Click += UndoMove;
            this.Controls.Add(undoButton);

            redoButton = new Button
            {
                Text = "Redo",
                Location = new Point(230, boardSize * 100 + 10),
                Size = new Size(100, 30)
            };
            redoButton.Click += RedoMove;
            this.Controls.Add(redoButton);

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    buttons[i, j] = new Button
                    {
                        Size = new Size(90, 90),
                        Location = new Point(j * 90 + 10, i * 90 + 10),
                        Font = new Font("Arial", 20)
                    };
                    buttons[i, j].Click += TileClick;
                    this.Controls.Add(buttons[i, j]);
                }
            }
        }

        private void UpdateUI()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                {
                    buttons[i, j].Text = board[i, j] == 0 ? "" : board[i, j].ToString();
                    buttons[i, j].BackColor = board[i, j] == 0 ? Color.LightGray : Color.White;
                }
            moveLabel.Text = $"Moves: {moveCount}";
        }

        private void TileClick(object sender, EventArgs e)
        {
            Button clicked = sender as Button;
            int row = (clicked.Top - 10) / 90;
            int col = (clicked.Left - 10) / 90;

            if ((Math.Abs(row - emptyRow) == 1 && col == emptyCol) ||
                (Math.Abs(col - emptyCol) == 1 && row == emptyRow))
            {
                int[,] newBoard = CloneBoard();
                newBoard[emptyRow, emptyCol] = newBoard[row, col];
                newBoard[row, col] = 0;

                currentNode.Children.Clear(); // clear redo history
                TreeNode child = new TreeNode(newBoard, currentNode);
                currentNode.Children.Add(child);
                currentNode = child;

                board = newBoard;
                emptyRow = row;
                emptyCol = col;
                moveCount++;
                UpdateUI();
                CheckWin();
            }
        }

        private int[,] CloneBoard()
        {
            return CloneBoard(board);
        }

        private int[,] CloneBoard(int[,] source)
        {
            int[,] copy = new int[boardSize, boardSize];
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    copy[i, j] = source[i, j];
            return copy;
        }

        private void LocateEmptyTile()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    if (board[i, j] == 0)
                    {
                        emptyRow = i;
                        emptyCol = j;
                        return;
                    }
        }

        private void UndoMove(object sender, EventArgs e)
        {
            if (currentNode.Parent != null)
            {
                currentNode = currentNode.Parent;
                board = CloneBoard(currentNode.State);
                LocateEmptyTile();
                moveCount = Math.Max(0, moveCount - 1);
                UpdateUI();
            }
            else
            {
                MessageBox.Show("No more moves to undo!", "Undo");
            }
        }

        private void RedoMove(object sender, EventArgs e)
        {
            if (currentNode.Children.Count > 0)
            {
                currentNode = currentNode.Children[0];
                board = CloneBoard(currentNode.State);
                LocateEmptyTile();
                moveCount++;
                UpdateUI();
            }
            else
            {
                MessageBox.Show("No more moves to redo!", "Redo");
            }
        }

        private void Move(string direction, bool updateTree)
        {
            int newRow = emptyRow, newCol = emptyCol;
            switch (direction)
            {
                case "up": newRow++; break;
                case "down": newRow--; break;
                case "left": newCol++; break;
                case "right": newCol--; break;
            }

            if (newRow >= 0 && newRow < boardSize && newCol >= 0 && newCol < boardSize)
            {
                board[emptyRow, emptyCol] = board[newRow, newCol];
                board[newRow, newCol] = 0;
                emptyRow = newRow;
                emptyCol = newCol;
            }
        }

        private void CheckWin()
        {
            int num = 1;
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                {
                    if (i == boardSize - 1 && j == boardSize - 1 && board[i, j] == 0) continue;
                    if (board[i, j] != num++) return;
                }

            stopwatch.Stop();
            TimeSpan timeTaken = stopwatch.Elapsed;
            string message = $"🎉 {playerName}, you solved it in {moveCount} moves!\nTime: {timeTaken:mm\\:ss}";
            MessageBox.Show(message, "Victory");

            File.AppendAllText("scores.txt",
                $"{DateTime.Now}: {playerName} | Level: {level} | Moves: {moveCount} | Time: {timeTaken:mm\\:ss}{Environment.NewLine}");
        }
    }
}
