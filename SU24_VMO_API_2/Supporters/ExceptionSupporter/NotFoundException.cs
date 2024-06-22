namespace SU24_VMO_API.Supporters.ExceptionSupporter
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }
        public NotFoundException(string msg) : base(msg) { }
        public NotFoundException(string msg, Exception inner) : base(msg, inner) { }
    }
}
