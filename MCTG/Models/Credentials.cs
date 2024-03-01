using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Models
{
    internal class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public override string ToString()
        {
            return $"Username: {Username}, Password(hash): {Password}";
        }
    }
}
