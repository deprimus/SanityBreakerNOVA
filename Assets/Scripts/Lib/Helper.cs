public static class Helper
{
    public static T ProducedOrDefault<T>(uint maxRetryCount, Delegates.ProducerDelegate<T> producer, Delegates.PredicateDelegate<T> predicate, T @default)
    {
        while(maxRetryCount-- > 0)
        {
            T value = producer();
            if (predicate(value))
                return value;
        }

        return @default;
    }
}