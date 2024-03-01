using MCTG.Interfaces;

namespace MCTG
{
    //Classes to handle Exceptions of different kinds, Inheritates from class Exception and implement Interface
    public class BadRequEx : Exception, IException
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; } = 400;
        public BadRequEx() 
        {
            ErrorMessage = "Request error";
        }
        public BadRequEx(string message) : base(message)  //provides message for base class Exeption
        {
            ErrorMessage = message;
        }
        public BadRequEx(string message, Exception inner) : base(message, inner)  //provides inner exeption
        {
            ErrorMessage = message;
        }
    }
    public class UnauthorizedEx : Exception, IException
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; } = 401;
        public UnauthorizedEx() 
        {
            ErrorMessage = "Not authorized";
        }
        public UnauthorizedEx(string message) : base(message)  //provides message for base class Exeption
        {
            ErrorMessage = message;
        }
        public UnauthorizedEx(string message, Exception inner) : base(message, inner) //provides inner exeption
        {
            ErrorMessage = message;
        }
    }
    public class ExistingEx : Exception, IException 
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; } = 402;
        public ExistingEx()
        {
            ErrorMessage = "Already existing";
        }
        public ExistingEx(string message) : base(message)
        {
            ErrorMessage = message;
        }
        public ExistingEx(string message, Exception inner) : base(message, inner)
        {
            ErrorMessage = message;
        }

    }
    public class AccsessEx : Exception, IException
    {
        public string ErrorMessage { get; }

        public int StatusCode { get; } = 403;

        public AccsessEx()
        {
            ErrorMessage = "Content not accessible";
        }

        public AccsessEx(string message) : base(message)
        {
            ErrorMessage = message;
        }

        public AccsessEx(string message, Exception inner) : base(message, inner)
        {
            ErrorMessage = message;
        }
    }
    public class NotFoundEx : Exception, IException
    {
        public string ErrorMessage { get; }

        public int StatusCode { get; } = 404;

        public NotFoundEx()
        {
            ErrorMessage = "Not found";
        }

        public NotFoundEx(string message) : base(message)
        {
            ErrorMessage = message;
        }

        public NotFoundEx(string message, Exception inner) : base(message, inner)
        {
            ErrorMessage = message;
        }
    }

    //what if no content /API Specification?

    public class NoContentFoundEx : Exception, IException
    {
        public string ErrorMessage { get; }
        public int StatusCode { get; } = 204;

        public NoContentFoundEx()
        {
            ErrorMessage = "Good Request but no Content";
        }
        public NoContentFoundEx(string message) : base(message)
        {
            ErrorMessage = message;
        }
        public NoContentFoundEx(string message, Exception inner) : base(message, inner)
        {
            ErrorMessage = message;
        }
    }


        //what about databse

    public class DatabaseEx : Exception, IException
    {
        public string ErrorMessage { get; }

        public int StatusCode { get; } = 503;

        public DatabaseEx()
        {
            ErrorMessage = "Server error";
        }

        public DatabaseEx(string message) : base(message)
        {
            ErrorMessage = message;
        }

        public DatabaseEx(string message, Exception inner) : base(message, inner)
        {
            ErrorMessage = message;
        }
    }
}
