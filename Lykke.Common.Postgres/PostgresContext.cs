using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Lykke.Common.Postgres
{
    public abstract class PostgresContext : DbContext
    {
        private string _connectionString;
        private readonly string _schema;
        public bool IsTraceEnabled { set; get; }
        
        /// <summary>
        /// Constructor used for migrations.
        /// </summary>
        /// <param name="schema">The schema which should be used.</param>
        public PostgresContext(string schema)
        {
            _schema = schema;
            
            IsTraceEnabled = true;
        }

        /// <summary>
        /// Constructor used for factories etc.
        /// </summary>
        /// <param name="schema">The schema which should be used.</param>
        /// <param name="connectionString">Connection string to the database.</param>
        /// <param name="isTraceEnabled">Whether or not display EF logs.</param>
        public PostgresContext(string schema, string connectionString, bool isTraceEnabled)
        {
            _schema = schema;
            _connectionString = connectionString;
            
            IsTraceEnabled = isTraceEnabled;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (IsTraceEnabled)
            {
                optionsBuilder.UseLoggerFactory(new LoggerFactory(new[] {new ConsoleLoggerProvider((_, __) => true, true)}));
            }
            
            // Manual connection string entry for migrations.
            if (_connectionString == null)
            {
                Console.Write("Enter connection string: ");

                _connectionString = Console.ReadLine();
            }
            
            optionsBuilder.UseNpgsql(
                _connectionString,
                x => x.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName, _schema));
            
            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}