namespace ASP.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class NotAllowedException : Exception
{
    public NotAllowedException(string message) : base(message) { }
}