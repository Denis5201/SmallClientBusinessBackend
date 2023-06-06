using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallClientBusiness.Common.Exceptions
{
    [Serializable]
    public class IncorrectDataException : Exception
    {
        public IncorrectDataException() { }

        public IncorrectDataException(string message) : base(message) { }

        public IncorrectDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
