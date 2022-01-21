using OnlineLibrary.Common.Exceptions.Enum;

namespace OnlineLibrary.Common.Exceptions
{
    public class OLBadRequest : OLException
    {
        public OLBadRequest(string message) : base(message)
        {
            Property = ExceptionType.BadRequest;
        }
    }
}
