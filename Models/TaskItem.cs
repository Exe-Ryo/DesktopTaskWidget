using System;

namespace DesktopTaskWidget.Models
{
    public class TaskItem
    {
        public long TaskId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

