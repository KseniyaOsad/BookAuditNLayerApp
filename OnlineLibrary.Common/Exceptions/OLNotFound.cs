using OnlineLibrary.Common.Exceptions.Enum;

namespace OnlineLibrary.Common.Exceptions
{
    public class OLNotFound : OLException
    {
        public OLNotFound(string message) : base(message)
        {
            Property = ExceptionType.NotFound;
        }
    }
}
