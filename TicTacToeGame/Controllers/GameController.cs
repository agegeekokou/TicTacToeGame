using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToeGame.Models;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Numerics;

namespace TicTacToeGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private static char[,] board = new char[3, 3];
        private static char currentPlayer = 'X';
        private static readonly Score score = new Score();
       

        public GameController()
        {
           
        }

        [HttpPost("init")]
        public IActionResult InitializeGame()
        {
            ResetBoard();
            currentPlayer = 'X';

            return Ok(new { board, currentPlayer, score });
        }

        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveAndModeRequest request)
        {
            if (request.PlayerVsComputer)
            {
                if (currentPlayer == 'X' && request.Row != null && request.Col != null)
                {
                    // Player's move                
                    if (board[(int)request.Row, (int)request.Col] == '\0')
                    {
                        board[(int)request.Row, (int)request.Col] = currentPlayer;

                        var (isWinPlayer, winningCellsPlayer) = CheckWin(board, currentPlayer);

                        // Check for a draw if no win
                        bool isDrawPlayer = !isWinPlayer && IsBoardFull(board);

                        // Update score if there's a win
                        if (isWinPlayer && currentPlayer == 'X')
                        {
                            score.PlayerX++;

                            var result = new
                            {
                                board,
                                currentPlayer = currentPlayer == 'X' ? 'O' : 'X',
                                isWinPlayer,
                                isDrawPlayer,
                                score,
                                winningCellsPlayer,
                                winner = "Player"
                            };

                            return Ok(result);
                        }

                        if (!isWinPlayer && !isDrawPlayer)
                        {
                            var result = new
                            {
                                board,
                                currentPlayer = currentPlayer == 'X' ? 'O' : 'X',
                                isWinPlayer,
                                isDrawPlayer,
                                score
                            };

                            currentPlayer = result.currentPlayer; // Switch turns only if no win or draw
                        }
                    }
                }
                // Computer's move
                //ComputerMove();
                if (currentPlayer == 'O')
                {
                    //ComputerMove();
                    var (row, col) = ComputerMove();
                    Console.WriteLine($"Computer move: Row={row}, Col={col}");

                    // Check for win/draw after computer's move
                    var (isWin, winningCells) = CheckWin(board, currentPlayer);

                    // Check for a draw if no win
                    bool isDraw = !isWin && IsBoardFull(board);

                    if (isWin && currentPlayer == 'O')
                    {
                        score.PlayerO++;

                        var result = new
                        {
                            board,
                            currentPlayer = currentPlayer == 'X' ? 'O' : 'X',
                            isWin,
                            isDraw,
                            row,
                            col,
                            score,
                            winningCells,
                            winner = "Computer"
                        };

                        return Ok(result);
                    }

                    if (!isWin && !isDraw)
                    {
                        var result = new
                        {
                            board,
                            currentPlayer = currentPlayer == 'X' ? 'O' : 'X',
                            isWin,
                            isDraw,
                            row,
                            col,
                            score
                        };

                        currentPlayer = result.currentPlayer; // Switch turns only if no win or draw

                        return Ok(result);
                    }
                }

                return BadRequest("Cell already occupied.");                
            }
            else
            {
                // Player vs Player logic
                if (request.Row != null && request.Col != null)
                {
                    if (board[(int)request.Row, (int)request.Col] == '\0')
                    {
                        board[(int)request.Row, (int)request.Col] = currentPlayer;

                        var (isWin, winningCells) = CheckWin(board, currentPlayer);

                        // Check for a draw if no win
                        bool isDraw = !isWin && IsBoardFull(board);

                        // Update score if there's a win
                        if (isWin)
                        {
                            if (currentPlayer == 'X')
                                score.PlayerX++;
                            else
                                score.PlayerO++;
                        }

                        var result = new
                        {
                            board,
                            currentPlayer = currentPlayer == 'X' ? 'O' : 'X',
                            isWin,
                            isDraw,
                            score,
                            winningCells
                        };


                        if (!isWin && !isDraw)
                            currentPlayer = result.currentPlayer; // Switch turns only if no win or draw

                        return Ok(result);
                    }
                }

                return BadRequest("Cell already occupied.");
            }
            
            //return BadRequest("Cell already occupied.");
        }

        [HttpPost("reset")]
        public IActionResult ResetGame()
        {
            ResetBoard();
            currentPlayer = 'X';

            return Ok(new { board, currentPlayer, score });
        }

        private void ResetBoard()
        {
            Array.Clear(board, 0, board.Length);
        }


        private (bool isWin, List<Dictionary<string, int>> winningCells) CheckWin(char[,] board, char player)
        {
            List<Dictionary<string, int>> winningCells = new List<Dictionary<string, int>>();

            // Check rows
            for (int row = 0; row < 3; row++)
            { 
                if (board[row, 0] == player && board[row, 1] == player && board[row, 2] == player)
                {
                    winningCells.Add(new Dictionary<string, int> { { "row", row }, { "col", 0 } });
                    winningCells.Add(new Dictionary<string, int> { { "row", row }, { "col", 1 } });
                    winningCells.Add(new Dictionary<string, int> { { "row", row }, { "col", 2 } });

                    return (true, winningCells);
                }
            }

            // Check columns
            for (int col = 0; col < 3; col++)
            {
                if (board[0, col] == player && board[1, col] == player && board[2, col] == player)
                {
                    winningCells.Add(new Dictionary<string, int> { { "row", 0 }, { "col", col } });
                    winningCells.Add(new Dictionary<string, int> { { "row", 1 }, { "col", col } });
                    winningCells.Add(new Dictionary<string, int> { { "row", 2 }, { "col", col } });

                    return (true, winningCells);
                }
            }

            // Check diagonals
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            {
                winningCells.Add(new Dictionary<string, int> { { "row", 0 }, { "col", 0 } });
                winningCells.Add(new Dictionary<string, int> { { "row", 1 }, { "col", 1 } });
                winningCells.Add(new Dictionary<string, int> { { "row", 2 }, { "col", 2 } });

                return (true, winningCells);
            }
            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            {
                winningCells.Add(new Dictionary<string, int> { { "row", 0 }, { "col", 2 } });
                winningCells.Add(new Dictionary<string, int> { { "row", 1 }, { "col", 1 } });
                winningCells.Add(new Dictionary<string, int> { { "row", 2 }, { "col", 0 } });

                return (true, winningCells);
            }

            return (false, winningCells);
        }

        // Helper method to check if the board is full (for a draw)
        private bool IsBoardFull(char[,] board)
        {
            foreach (char cell in board)
            {
                if (cell == '\0') return false;
            }
            return true;
        }

        //private static (int selectedRow, int selectedCol) ComputerMove()
        //{
        //    // Find all empty cells on the board
        //    List<Dictionary<string, int>> emptyCells = new List<Dictionary<string, int>>();
        //    int selectedRow = 0;
        //    int selectedCol = 0;

        //    for (int row = 0; row < 3; row++)
        //    {
        //        for (int col = 0; col < 3; col++)
        //        {
        //            if (board[row, col] == '\0') // Check if the cell is empty
        //            {
        //                emptyCells.Add(new Dictionary<string, int> { { "row", row }, { "col", col } });
        //            }
        //        }
        //    }

        //    // Check if there are any empty cells left
        //    if (emptyCells.Count > 0)
        //    {
        //        // Select a random cell for the computer's move
        //        Random random = new Random();
        //        int randomIndex = random.Next(emptyCells.Count);
        //        selectedRow = emptyCells[randomIndex]["row"];
        //        selectedCol = emptyCells[randomIndex]["col"];

        //        // Place 'O' (computer's move) on the board
        //        board[selectedRow, selectedCol] = 'O';

        //        return (selectedRow, selectedCol);
        //    }

        //    return (selectedRow, selectedCol);
        //}

        //private static (int selectedRow, int selectedCol) ComputerMove()
        //{
        //    // Find all empty cells on the board
        //    List<(int row, int col)> emptyCells = new List<(int row, int col)>();

        //    for (int row = 0; row < 3; row++)
        //    {
        //        for (int col = 0; col < 3; col++)
        //        {
        //            if (board[row, col] == '\0') // Check if the cell is empty
        //            {
        //                emptyCells.Add((row, col));
        //            }
        //        }
        //    }

        //    // Check if there are any empty cells left
        //    if (emptyCells.Count > 0)
        //    {
        //        // Select a random empty cell
        //        Random random = new Random();
        //        int randomIndex = random.Next(emptyCells.Count);

                // Get the row and column of the selected cell
        //        var (selectedRow, selectedCol) = emptyCells[randomIndex];

        //        // Place 'O' (computer's move) on the board
        //        board[selectedRow, selectedCol] = 'O';

        //        Console.WriteLine($"Computer moved to: Row {selectedRow}, Col {selectedCol}");

        //        return (selectedRow, selectedCol);
        //    }
        //    else
        //    {
        //        Console.WriteLine("No empty cells available for the computer to move.");

        //        return (0, 0);
        //    }
        //}

        private (int Row, int Col) ComputerMove()
        {
            return GetBestMove();
        }

        private (int Row, int Col) GetBestMove()
        {
            int bestScore = int.MinValue;
            (int Row, int Col) bestMove = (-1, -1);

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == '\0')
                    {
                        board[row, col] = 'O'; // Computer's move
                        int score = Minimax(board, 0, false);
                        board[row, col] = '\0'; // Undo the move

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (row, col);
                        }
                    }
                }
            }

            board[bestMove.Row, bestMove.Col] = 'O'; // Apply the best move to the board
            return bestMove;
        }

        private int Minimax(char[,] board, int depth, bool isMaximizing)
        {
            //var winner = CheckWin(board, currentPlayer);
            var (isWin, winningCells) = CheckWin(board, currentPlayer);
            var (isWinPlayer, winningCellsPlayer) = CheckWin(board, currentPlayer);
            //if (winner == 'O') return 10 - depth; // Computer wins
            if (isWin == true && currentPlayer == 'O')
            {
                return 10 - depth; // Computer wins
            }

            if (isWinPlayer == true && currentPlayer == 'X')
            {
                return depth - 10; // Player wins
            }
            //if (winner == 'X') return depth - 10; // Player wins

            if ((!isWin && IsBoardFull(board)) || (!isWinPlayer && IsBoardFull(board))) return 0; // Draw

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (board[row, col] == '\0')
                        {
                            board[row, col] = 'O';
                            int score = Minimax(board, depth + 1, false);
                            board[row, col] = '\0';
                            bestScore = Math.Max(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int row = 0; row < 3; row++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        if (board[row, col] == '\0')
                        {
                            board[row, col] = 'X';
                            int score = Minimax(board, depth + 1, true);
                            board[row, col] = '\0';
                            bestScore = Math.Min(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
        }
    }
}
