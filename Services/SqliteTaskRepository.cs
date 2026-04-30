using System;
using System.Collections.Generic;
using DesktopTaskWidget.Models;
using Microsoft.Data.Sqlite;

namespace DesktopTaskWidget.Services
{
    public class SqliteTaskRepository : ITaskRepository
    {
        private readonly string _databasePath;

        public SqliteTaskRepository(string databasePath)
        {
            _databasePath = databasePath;
        }

        public void Initialize()
        {
            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
CREATE TABLE IF NOT EXISTS tasks (
    task_id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    is_completed INTEGER NOT NULL DEFAULT 0,
    due_date TEXT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL,
    completed_at TEXT NULL
);
CREATE INDEX IF NOT EXISTS idx_tasks_completed_due ON tasks (is_completed, due_date);";
                command.ExecuteNonQuery();
            }
        }

        public IReadOnlyList<TaskItem> GetTasks(bool includeCompleted)
        {
            var tasks = new List<TaskItem>();

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
SELECT task_id, title, is_completed, due_date, sort_order, created_at, updated_at, completed_at
FROM tasks
WHERE $includeCompleted = 1 OR is_completed = 0
ORDER BY is_completed ASC,
         CASE WHEN due_date IS NULL THEN 1 ELSE 0 END ASC,
         due_date ASC,
         sort_order ASC,
         created_at ASC;";
                command.Parameters.AddWithValue("$includeCompleted", includeCompleted ? 1 : 0);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new TaskItem
                        {
                            TaskId = reader.GetInt64(0),
                            Title = reader.GetString(1),
                            IsCompleted = reader.GetInt32(2) == 1,
                            DueDate = ReadNullableDate(reader, 3),
                            SortOrder = reader.GetInt32(4),
                            CreatedAt = DateTime.Parse(reader.GetString(5)),
                            UpdatedAt = DateTime.Parse(reader.GetString(6)),
                            CompletedAt = ReadNullableDate(reader, 7)
                        });
                    }
                }
            }

            return tasks;
        }

        public void AddTask(string title, DateTime? dueDate)
        {
            var now = DateTime.UtcNow;

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
INSERT INTO tasks (title, is_completed, due_date, sort_order, created_at, updated_at)
VALUES ($title, 0, $dueDate, 0, $createdAt, $updatedAt);";
                command.Parameters.AddWithValue("$title", title);
                command.Parameters.AddWithValue("$dueDate", ToDbValue(dueDate));
                command.Parameters.AddWithValue("$createdAt", now.ToString("O"));
                command.Parameters.AddWithValue("$updatedAt", now.ToString("O"));
                command.ExecuteNonQuery();
            }
        }

        public void SetCompleted(long taskId, bool isCompleted)
        {
            var now = DateTime.UtcNow;

            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
UPDATE tasks
SET is_completed = $isCompleted,
    completed_at = $completedAt,
    updated_at = $updatedAt
WHERE task_id = $taskId;";
                command.Parameters.AddWithValue("$isCompleted", isCompleted ? 1 : 0);
                command.Parameters.AddWithValue("$completedAt", isCompleted ? (object)now.ToString("O") : DBNull.Value);
                command.Parameters.AddWithValue("$updatedAt", now.ToString("O"));
                command.Parameters.AddWithValue("$taskId", taskId);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteTask(long taskId)
        {
            using (var connection = OpenConnection())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM tasks WHERE task_id = $taskId;";
                command.Parameters.AddWithValue("$taskId", taskId);
                command.ExecuteNonQuery();
            }
        }

        private SqliteConnection OpenConnection()
        {
            AppPaths.EnsureDirectories();
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath
            };
            var connection = new SqliteConnection(builder.ToString());
            connection.Open();
            return connection;
        }

        private static object ToDbValue(DateTime? value)
        {
            return value.HasValue ? (object)value.Value.Date.ToString("O") : DBNull.Value;
        }

        private static DateTime? ReadNullableDate(SqliteDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return DateTime.Parse(reader.GetString(ordinal));
        }
    }
}

