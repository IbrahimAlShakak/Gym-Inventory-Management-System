namespace Gym_Inventory_Management_System.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class InvalidOperationBusinessException : Exception
{
    public InvalidOperationBusinessException(string message) : base(message) { }
}

public class InvalidLoginInput : Exception
{
    public InvalidLoginInput(string message) : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}