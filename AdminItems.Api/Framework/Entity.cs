using CSharpFunctionalExtensions;

namespace AdminItems.Api.Framework;

public class Entity<TId, TVersion, TValue, TEntity> : Entity<TId>
    where TEntity : Entity<TId, TVersion, TValue, TEntity>
{
    public TVersion Version { get; }
    public TValue Value { get; protected set; }

    public Entity(TId id, TVersion version, TValue value):base(id)
    {
        Version = version;
        Value = value;
    }
    
    public TEntity WithValue(TValue value)
    {
        Value = value;
        return (TEntity)this;
    }

    public void Deconstruct(out TId id,  out TVersion version, out TValue value)
    {
        id = Id;
        version = Version;
        value = Value;
    }
}