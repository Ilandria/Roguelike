using System;
using System.Collections;

public interface ILoadable
{
    IEnumerator Load(Action<float, string> progress);
}
