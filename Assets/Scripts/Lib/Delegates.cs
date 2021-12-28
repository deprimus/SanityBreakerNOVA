using System.Collections;
using System.Collections.Generic;

public class Delegates
{
    public delegate void ShallowDelegate();
    public delegate T ProducerDelegate<T>();
    public delegate T ProducerDelegate<T, U>(U u);
    public delegate T ProducerDelegate<T, U, V>(U u, V v);
    public delegate T ProducerDelegate<T, U, V, W>(U u, V v, W w);
    public delegate bool PredicateDelegate<T>(T who);
}
