using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// class BoardDTO
    /// represents a record in table Boards in DB(BoardID,BoardName,CreatorEmail)
    /// </summary>
    class BoardDTO:DTO
    {
        /// <summary>
        /// Boards table in DB-column names:
        /// </summary>
        public const string BoardNameColumnName = "BoardName";
        public const string CreatorEmailColumnName = "CreatorEmail";
        public const string IDColumnName = "ID";

        /// <summary>
        /// board creator email field and its getter & setter
        /// </summary>
        private string _email;
        public string Email { get => _email; private set {_controller.Update(_primaryKeys,_primaryVals, CreatorEmailColumnName, value); } }
        /// <summary>
        /// board name field and its getter & setter
        /// </summary>
        private string _boardName;
        public string BoardName { get => _boardName; private set { _boardName = value; _controller.Update(_primaryKeys,_primaryVals, BoardNameColumnName, value); } }
        /// <summary>
        /// Board id field and its getter
        /// </summary>
        private long _Id;
        public long ID { get => _Id; }

        /// <summary>
        /// BoardDTO constructor
        /// </summary>
        /// <param name="ID">board id</param>
        /// <param name="email">board creator email</param>
        /// <param name="boardName">board name</param>
        public BoardDTO(long ID, string email, string boardName) : base(new BoardDalController(),new string[] {IDColumnName},new Object[] { ID})
        {
            _Id = ID;
            _email = email;
            _boardName = boardName;
        }
    }
}
