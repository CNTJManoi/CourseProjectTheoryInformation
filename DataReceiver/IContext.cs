using System;

namespace DataReceiver;

public interface IContext
{
    bool IsSynchronized { get; }
    void Invoke(Action action);
    void BeginInvoke(Action action);
}