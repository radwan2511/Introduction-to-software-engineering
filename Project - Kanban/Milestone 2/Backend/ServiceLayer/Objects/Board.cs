using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct Board
    {
        public readonly string creatorEmail;
        public readonly string boardName;
        public readonly Column backlog;
        public readonly Column in_progress;
        public readonly Column done;
        /// <summary>
        /// Board(Service object) constructor
        /// converts a Board from the Businees Layer to a new Service Layer Board
        /// </summary>
        /// <param name="board">Business layer board to be converted</param>
        internal Board(BusinessLayer.Board board)
        {
            creatorEmail = board.CreatorEmail;
            boardName = board.Name;
            backlog = new Column(board.GetColumn(0).tasks,board.GetColumn(0).tasksLimit,"backlog");
            in_progress = new Column(board.GetColumn(1).tasks, board.GetColumn(1).tasksLimit, "in progress");
            done = new Column(board.GetColumn(2).tasks, board.GetColumn(2).tasksLimit, "done");
        }
    }
}
