using System;

namespace Lykke.Common.Postgres
{
    public class PostgresContextFactory<T>
    {
        private readonly Func<T> _contextCreator;

        public PostgresContextFactory(Func<T> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public T CreateDataContext()
        {
            return _contextCreator.Invoke();
        }
    }
}