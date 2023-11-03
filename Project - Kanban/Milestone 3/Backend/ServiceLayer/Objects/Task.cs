using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Task
    {
        public readonly int Id;
        public readonly DateTime CreationTime;
        public readonly string Title;
        public readonly string Description;
        public readonly DateTime DueDate;
        public readonly string emailAssignee;
        public readonly string boardName;
        public readonly string creatorEmail;
        public readonly int columnOrdinal;
       internal Task(int id, DateTime creationTime, string title, string description, DateTime DueDate, string emailAssignee)
        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.emailAssignee = emailAssignee;
            this.columnOrdinal = 0;
            this.creatorEmail = "";
            this.boardName = "";
        }
        internal Task(int colOrd,int id, DateTime creationTime, string title, string description, DateTime DueDate, string emailAssignee,string bName,string cEmail)
        {
            this.Id = id;
            this.columnOrdinal = colOrd;
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.emailAssignee = emailAssignee;
            this.creatorEmail = cEmail;
            this.boardName = bName;
        }
    }
}
