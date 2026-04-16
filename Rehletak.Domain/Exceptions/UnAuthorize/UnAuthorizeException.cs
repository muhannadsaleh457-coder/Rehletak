using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Domain.Exceptions.UnAuthorize
{
    public class UnAuthorizeException(string message) : Exception(message)
    {
    }
}
