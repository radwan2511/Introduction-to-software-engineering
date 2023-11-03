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
        public readonly IList<Column> columns;
        /// <summary>
        /// Board(Service object) constructor
        /// converts a Board from the Businees Layer to a new Service Layer Board
        /// </summary>
        /// <param name="board">Business layer board to be converted</param>
        internal Board(BusinessLayer.Board board)
        {
            creatorEmail = board.CreatorEmail;
            boardName = board.Name;
            columns = new List<Column>();
            for (int i = 0; i < board.Columns.Count; i++)
            {
                columns.Add(new Column(creatorEmail,boardName,board.Columns[i].tasks, board.Columns[i].tasksLimit, board.Columns[i].Name,i));
            }
        }
    }
}

