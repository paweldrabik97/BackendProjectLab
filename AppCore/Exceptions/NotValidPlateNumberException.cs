namespace AppCore.Exceptions;

public class NotValidPlateNumberException : Exception
{
    public NotValidPlateNumberException(string? message) : base(message)
    {
    }
}