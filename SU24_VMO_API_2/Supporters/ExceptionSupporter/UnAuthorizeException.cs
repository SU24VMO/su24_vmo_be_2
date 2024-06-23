namespace SU24_VMO_API_2.Supporters.ExceptionSupporter
{
    public class UnAuthorizeException : Exception
    {
        public UnAuthorizeException() : base() { }
        public UnAuthorizeException(string msg) : base(msg) { }
        public UnAuthorizeException(string msg, Exception inner) : base(msg, inner) { }
    }
}
