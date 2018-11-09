using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Core.Exceptions
{
    public class RepositoryException:Exception
    {
        public RepositoryException(string message) : base(message)
        {

        }
        public RepositoryException(string message, Exception innerException):base(message, innerException)
        {

        }
    }
}
