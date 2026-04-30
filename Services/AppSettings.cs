using System.Runtime.Serialization;

namespace DesktopTaskWidget.Services
{
    [DataContract]
    public class AppSettings
    {
        [DataMember]
        public double Width { get; set; } = 340;

        [DataMember]
        public double Height { get; set; } = 440;

        [DataMember]
        public bool ShowCompletedTasks { get; set; }

        [DataMember]
        public bool ShowWidgetOnStartup { get; set; } = true;
    }
}

