using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    internal class UserStats
    {
        public string? Username { get; set; }
        public int Elo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Coins { get; set; }
        public int? UserId { get; set; }
        public int Draws { get; set; }
        public int Played { get; set; }
        public override string ToString()
        {
            return $"Elo: {Elo}, Wins: {Wins}, Losses: {Losses}";
        }
    }
}
