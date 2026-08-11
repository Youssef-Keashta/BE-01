using BE_01.Models;
using Microsoft.Data.SqlClient;

namespace BE_01.Data
{
    public static class TaskDatabase
    {
        private static string _connectionString = "";

        public static void Initialize(string connectionString)
        {
            _connectionString = connectionString;

            EnsureDatabaseExists(connectionString);

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tasks' AND xtype='U')
        CREATE TABLE tasks (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            Title NVARCHAR(255) NOT NULL,
            Done BIT NOT NULL DEFAULT 0
        );";
            createTableCommand.ExecuteNonQuery();

            SeedIfEmpty(connection);
        }

        private static void EnsureDatabaseExists(string targetConnectionString)
        {
            var builder = new SqlConnectionStringBuilder(targetConnectionString);
            string databaseName = builder.InitialCatalog;

            // Connect to the server without targeting a specific database
            builder.InitialCatalog = "master";

            using var connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $@"
        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
        CREATE DATABASE [{databaseName}];";
            command.ExecuteNonQuery();
        }

        private static void SeedIfEmpty(SqlConnection connection)
        {
            var countCommand = connection.CreateCommand();
            countCommand.CommandText = "SELECT COUNT(*) FROM tasks";
            int count = (int)countCommand.ExecuteScalar();

            if (count == 0)
            {
                var seedCommand = connection.CreateCommand();
                seedCommand.CommandText = @"
                    INSERT INTO tasks (Title, Done) VALUES ('Buy Water', 0);
                    INSERT INTO tasks (Title, Done) VALUES ('Watch Movie', 0);
                    INSERT INTO tasks (Title, Done) VALUES ('Take Out the Trash', 0);";
                seedCommand.ExecuteNonQuery();
            }
        }

        public static List<ToDoTask> GetAll()
        {
            var tasks = new List<ToDoTask>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Done FROM tasks";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(ToDoTask.FromDatabase(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetBoolean(2)
                ));
            }

            return tasks;
        }

        public static ToDoTask? GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Done FROM tasks WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return ToDoTask.FromDatabase(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetBoolean(2)
                );
            }

            return null;
        }

        public static ToDoTask Insert(string title)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO tasks (Title, Done) VALUES (@title, 0);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            command.Parameters.AddWithValue("@title", title);

            int newId = (int)command.ExecuteScalar();

            return ToDoTask.FromDatabase(newId, title, false);
        }
    }
}