using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    class Task
    {
        private int Id;
        private DateTime CreationTime;
        private string Title;
        private string Description;
        private DateTime DueDate;

        private Boolean isDone;

        /*
        Task constructor

        input: int id, DateTime creationTime, string title, string description, DateTime DueDate
        output: none
         */
        public Task(int id, string title, string description, DateTime DueDate)
        {
            if (title == null || (title.Length == 0 || title.Length > 50)){
                throw new Exception("title should be not empty and should have max. 50 chars if is defined");
            }
            if (description != null && (description.Length > 300))
            {
                throw new Exception("description should have max. 300 chars if is defined");
            }
            if (DueDate < DateTime.Now)
            {
                throw new Exception("the given due date has already passed");
            }
            this.Id = id;
            this.CreationTime = DateTime.Now;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;

            this.isDone = false;
        }

        /*
        updating dueDate

        input: DateTime DueDate
        output: none
         */
        public void setDueDate(DateTime DueDate)
        {
            if (DueDate < DateTime.Now)
            {
                throw new Exception("the given due date has already passed");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            this.DueDate = DueDate;
        }

        /*
        updating title

        input: string title
        output: none
         */
        public void setTitle(string title)
        {
            if (title == null || (title.Length == 0 || title.Length > 50))
            {
                throw new Exception("title should be not empty and should have max. 50 chars if is defined");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            this.Title = title;
        }



        /*
       updating description

       input: string description
       output: none
        */
        public void setDescription(string description)
        {
            if (description != null && (description.Length > 300))
            {
                throw new Exception("description should have max. 300 chars if is defined");
            }
            if (isDone)
            {
                throw new Exception("a task which is done cannot be changed");
            }
            this.Description = description;
        }


        /*
        updating that the task is done

       input: none
       output: none
        */
        public void setDone()
        {
            this.isDone = true;
        }

        /*
        returning the task current status (isDone)

       input: none
       output: Boolean isDone
        */
        public Boolean getDone()
        {
            return this.isDone;
        }

        /*
        returning the tasks' id

       input: none
       output: int id
        */
        public int getId()
        {
            return Id;
        }

        /*
        returning the tasks' due date

       input: none
       output: DateTime due date
        */
        public DateTime getDueDate()
        {
            return DueDate;
        }

        /*
        returning the tasks' creation time

       input: none
       output: DateTime creation time
        */
        public DateTime getCreationTime()
        {
            return CreationTime;
        }

        /*
        returning the tasks' title

       input: none
       output: string title
        */

        public string getTitle()
        {
            return Title;
        }

        /*
        returning the tasks' description

       input: none
       output: string description
        */
        public string getDescription()
        {
            return Description;
        }
    }
}
