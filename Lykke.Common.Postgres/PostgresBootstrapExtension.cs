using System;
using Autofac;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Common.Postgres
{
    public static class PostgresBootstrapExtension
    {
        public static void RegisterPostgres<T>(this ContainerBuilder builder, Func<T> contextCreator) where T: PostgresContext, new()
        {
            using (var context = contextCreator.Invoke())
            {
                context.IsTraceEnabled = true;
                
                context.Database.Migrate();
            }
            
            builder.RegisterInstance(new PostgresContextFactory<T>(contextCreator))
                .AsSelf()
                .SingleInstance();
        }
    }
}