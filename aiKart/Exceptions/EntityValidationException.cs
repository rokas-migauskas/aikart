using aiKart.Interfaces;

namespace aiKart.Exceptions;
public class EntityValidationException<T> : Exception where T : class, IEntity
{
    public T Entity { get; }

    public EntityValidationException(T entity, string message) : base(message)
    {
        Entity = entity;
    }
}
