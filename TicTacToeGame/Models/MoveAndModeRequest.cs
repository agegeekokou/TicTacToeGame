namespace TicTacToeGame.Models
{
    public class MoveAndModeRequest
    {
        public int? Row { get; set; }
        public int? Col { get; set; }
        public bool PlayerVsPlayer { get; set; }
        public bool PlayerVsComputer { get; set; }
    }
}
