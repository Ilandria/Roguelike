using System;
using System.Collections;

public interface ILoadable
{
    bool IsLoaded { get; }
    IEnumerator Load(Action<float, string> progress);
}
