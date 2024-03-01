using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG.Interfaces
{
    internal interface IException  //Interface Class
    {
        string ErrorMessage {  get; }
        int StatusCode { get; }
    }
}
