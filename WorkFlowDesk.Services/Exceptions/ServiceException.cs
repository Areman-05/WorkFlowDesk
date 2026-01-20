namespace WorkFlowDesk.Services.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string message) : base(message)
    {
    }

    public ServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class EntityNotFoundException : ServiceException
{
    public EntityNotFoundException(string entityName, int id) 
        : base($"{entityName} con ID {id} no encontrado.")
    {
    }
}

public class ValidationException : ServiceException
{
    public ValidationException(string message) : base(message)
    {
    }
}
