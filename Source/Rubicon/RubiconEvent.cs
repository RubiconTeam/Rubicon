using System.Collections.Generic;

namespace Rubicon;

[GlobalClass] public partial class RubiconEvent : RefCounted
{
    private HashSet<Callable> _callables = new();
    
    public void Add(Callable callable)
    {
        _callables.Add(callable);
    }

    public bool Contains(Callable callable)
    {
        return _callables.Contains(callable);
    }

    public void Remove(Callable callable)
    {
        _callables.Remove(callable);
    }
    
    public int Count => _callables.Count;

    public Variant[] Invoke(params Variant[] args)
    {
        List<Variant> results = new List<Variant>();
        foreach (Callable callable in _callables)
            results.Add(callable.Call(args));
        
        return results.ToArray();
    }
}