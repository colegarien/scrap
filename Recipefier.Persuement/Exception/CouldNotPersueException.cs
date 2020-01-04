namespace Recipefier.Persuement.Exception
{
    public class CouldNotPersueException : System.Exception
    {
        public CouldNotPersueException() : base() {}

        public CouldNotPersueException(string message) : base(message) {}

        public CouldNotPersueException(string message, System.Exception innerException) : base(message, innerException){}
    }
}
