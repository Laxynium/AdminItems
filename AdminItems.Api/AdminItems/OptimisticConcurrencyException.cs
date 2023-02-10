namespace AdminItems.Api.AdminItems;

public class OptimisticConcurrencyException : Exception
{
    public OptimisticConcurrencyException(string operation, string entityName, string entityId) : base(
        $"Optimistic concurrency error occured during {operation} of the entity {entityName} with id={entityId}")
    {
        
    }
}