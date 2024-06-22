namespace SU24_VMO_API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DBTransactionAttribute : Attribute
    {
    }
}
