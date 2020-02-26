using System;

namespace Example.Model
{
    public class Log : IWithId
    {
        public int Id { get; set; }
        
        public User User { get; set; }
        
        public DateTimeOffset DateTime { get; set; }
        
        public LogType LogType { get; set; }

        public string ComponentType { get; set; }
        
        public string ComponentDescription { get; set; }
        
        public string Message { get; set; }
        
        public int ThreadId { get; set; }
    }
}