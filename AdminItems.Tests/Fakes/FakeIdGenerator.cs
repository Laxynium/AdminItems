namespace AdminItems.Tests.Fakes;

internal class FakeIdGenerator<T>
{
    private readonly Queue<T> _queue = new();

    public T Next()
    {
        if (!_queue.Any())
        {
            throw new InvalidOperationException("Cannot get next id, since queue is empty");
        }

        return _queue.Dequeue();
    }

    public void Enqueue(T id)
    {
        _queue.Enqueue(id);
    }
}