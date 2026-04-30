using System;
using System.Collections.Generic;
using DesktopTaskWidget.Models;

namespace DesktopTaskWidget.Services
{
    public interface ITaskRepository
    {
        void Initialize();
        IReadOnlyList<TaskItem> GetTasks(bool includeCompleted);
        void AddTask(string title, DateTime? dueDate);
        void SetCompleted(long taskId, bool isCompleted);
        void DeleteTask(long taskId);
    }
}

