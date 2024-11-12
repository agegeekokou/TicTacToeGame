﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicTacToeGame.Models;

namespace TicTacToeGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private static char[,] board = new char[3, 3];
        private static char currentPlayer = 'X';
        private static Score score = new Score(); 

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
        public IActionResult MakeMove([FromBody] MoveRequest move)
        {
            if (board[move.Row, move.Col] == '\0')
            {
                board[move.Row, move.Col] = currentPlayer;
                bool isWin = CheckWin(board, currentPlayer);
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
                    score
                };

                if (!isWin && !isDraw)
                   currentPlayer = result.currentPlayer; // Switch turns only if no win or draw

                return Ok(result);
            }

            return BadRequest("Cell already occupied.");
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


        private bool CheckWin(char[,] board, char player)
        {
            // Check rows
            for (int row = 0; row < 3; row++)
            {
                if (board[row, 0] == player && board[row, 1] == player && board[row, 2] == player)
                {
                    return true;
                }
            }

            // Check columns
            for (int col = 0; col < 3; col++)
            {
                if (board[0, col] == player && board[1, col] == player && board[2, col] == player)
                {
                    return true;
                }
            }

            // Check diagonals
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
            {
                return true;
            }
            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
            {
                return true;
            }

            // If no win found
            return false;
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


    }
}