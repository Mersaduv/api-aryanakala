using System;

namespace ApiAryanakala.Entities.Exceptions
{
    public class CoreException : Exception
    {
        public CoreException() : base() { }

        public CoreException(string message) : base(message)
        {
        }
    }
}
