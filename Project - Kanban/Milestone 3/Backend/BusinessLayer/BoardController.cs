using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Board Controller tasks
    /// manages all the boards in the kanban system
    /// </summary>
    class BoardController
    {
        /// <summary>
        /// Boards field contains pairs of a board and a list of board member emails
        /// </summary>
        private readonly Dictionary<Board, IList<string>> Boards;
        /// <summary>
        /// Connected User field contains the email of the connected user(if exists)
        /// </summary>
        private readonly IList<string> ConnectedUser;
        private BoardDalController boardDalController;
        private BoardMemberDalController boardMemberDalController;
        private int IdCounter;

        /// <summary>
        /// Board controller constructor
        /// </summary>
        /// <param name="empty">An empty list which is created in the service layer and is joint with the
        /// user controller. This list will hold the current connected user</param>
        public BoardController(IList<string> empty)
        {
            Boards = new Dictionary<Board, IList<string>>();
            /* the idea: both user-controller and board-controller will hold
            joint list which contains the email of the connected User
            so the board-controller can know whether a user is logged in or not
            without needing aggregating other classes */
            if (empty == null || empty.Count != 0)
            {
                throw new Exception("we expect the list to be empty");
            }
            ConnectedUser = empty;
            IdCounter = 1;
            boardDalController = new BoardDalController();
            boardMemberDalController = new BoardMemberDalController();
        }

        /// <summary>
        /// Adds a new board to the system.
        /// </summary>
        /// <param name="email">creator email, should be logged in</param>
        /// <param name="boardName">the board name, should be unique among the given users' board names</param>
        public void AddBoard(string email, string boardName)
        {
            if (BoardExists(email, boardName))
            {
                throw new Exception("Board already exists");
            }
            if (!ConnectedUser.Contains(email))
            {
                throw new Exception("In order to add a board, the user should be connected");
            }
            IList<string> l = new List<string>();
            l.Add(email);
            Boards.Add(new Board(IdCounter, email, boardName), l);
            boardDalController.Insert(new BoardDTO(IdCounter, email, boardName));
            boardMemberDalController.Insert(new BoardMemberDTO(email, IdCounter, 1));
            IdCounter = IdCounter + 1;
        }

        /// <summary>
        /// Removes a board from the system.
        /// </summary>
        /// <param name="userEmail">email of the user who attempts to delete the board, 
        /// should be logged in and be the creator of the specified board</param>
        /// <param name="creatorEmail">board creator email</param>
        /// <param name="boardName">name of the board, there should be a board named like that by the given creator</param>
        public void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to remove a board, the user should be connected");
            }
            if (!BoardExists(creatorEmail, boardName))
            {
                throw new Exception("Board doesn't exist");
            }
            if (userEmail != creatorEmail)
            {
                throw new Exception("only the board creator can remove his/her board");
            }
            int id = 0;
            foreach (Board b in Boards.Keys)
            {
                if (b.CreatorEmail == creatorEmail && b.Name == boardName)
                {
                    boardMemberDalController.DeleteBoard(new BoardDTO(b.Id, creatorEmail, boardName));
                    id = b.Id;
                    Boards.Remove(b);
                    break;
                }
            }
            boardDalController.Delete(new BoardDTO(id, creatorEmail, boardName));
            new ColumnDalController().DeleteBoard(new BoardDTO(id, creatorEmail, boardName));
            new TaskDalController().DeleteBoard(new BoardDTO(id, creatorEmail, boardName));
        }


        /// <summary>
        /// finds in-progress tasks assigned to a given user from all the boards in the system.
        /// </summary>
        /// <param name="email">the assigned user email, should be logged in</param>
        /// <returns>list of all the in progress tasks assigned to the given user</returns>
        public IList<ITask> InProgressTasks(string email)
        {
            if (!ConnectedUser.Contains(email))
            {
                throw new Exception("In order to get in progress tasks, the user should be connected");
            }

            IList<ITask> inProgress = new List<ITask>();
            foreach (KeyValuePair<Board, IList<string>> pair in Boards)
            {
                IList<string> contributors = pair.Value;
                if (contributors.Contains(email))
                {
                    IList<ITask> boardInProgress = pair.Key.GetInProgressTasks(email);
                    foreach (ITask t in boardInProgress)
                    {
                        if (t.Assignee == email)
                        {
                            inProgress.Add(t);
                        }
                    }
                }
            }
            return inProgress;
        }


        /// <summary>
        /// a specific board getter
        /// </summary>
        /// <param name="userEmail">email of current user, should be logged-in and be a board member</param>
        /// <param name="creatorEmail">board creator email</param>
        /// <param name="boardName">the board name, there should be a board named like that by the given creator</param>
        /// <returns>the specified board</returns>
        public Board GetBoard(string userEmail, string creatorEmail, string boardName)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to access a board, the user should be connected");
            }
            foreach (Board b in Boards.Keys)
            {
                if (b.CreatorEmail == creatorEmail && b.Name == boardName)
                {
                    if (Boards[b].Contains(userEmail))
                        return b;
                    throw new Exception("in order to access a board, the user must be its creator/join it");
                }
            }
            throw new Exception("Board doesn't exist");
        }


        /// <summary>
        /// joins a user to a board
        /// </summary>
        /// <param name="userEmail">the user of the "joiner", should be logged-in and not be already a board member</param>
        /// <param name="creatorEmail">board creator email</param>
        /// <param name="boardName">he board name, there should be a board named like that by the given creator</param>
        public void JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to join a board, the user should be connected");
            }
            if (!BoardExists(creatorEmail, boardName))
            {
                throw new Exception("Board doesn't exist");
            }
            Board board = null;
            foreach (Board b in Boards.Keys)
            {
                if (b.CreatorEmail == creatorEmail && b.Name == boardName)
                {
                    board = b;
                    break;
                }
            }
            if (board == null)
            {
                throw new Exception("board doesn't exist");
            }
            Boards[board].Add(userEmail);
            boardMemberDalController.Insert(new BoardMemberDTO(userEmail, board.Id, Boards[board].Count + 1));
        }


        /// <summary>
        /// checks if there exists a specific board in the system
        /// </summary>
        /// <param name="email">board creator email</param>
        /// <param name="boardName">board name</param>
        /// <returns>true if board exists, otherwise false</returns>
        private bool BoardExists(string email, string boardName)
        {
            foreach (Board b in Boards.Keys)
            {
                if (b.CreatorEmail == email && b.Name == boardName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// getter for all board names where a specific user is a member of them
        /// </summary>
        /// <param name="userEmail">the user email, should be logged-in</param>
        /// <returns>a list with all the names of boards the user is a member of them</returns>
        public IList<string> GetBoardNames(string userEmail)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to join a board, the user should be connected");
            }
            IList<string> ans = new List<string>();
            foreach (Board b in Boards.Keys)
            {
                if (Boards[b].Contains(userEmail))
                {
                    ans.Add(b.Name);
                }
            }
            return ans;
        }

        /// <summary>
        /// deletes all the persisted board data(including columns and tasks) from the DB
        /// </summary>
        public void Delete()
        {
            boardDalController.DeleteAll();
            new ColumnDalController().DeleteAll();
            boardMemberDalController.DeleteAll();
            new TaskDalController().DeleteAll();
        }

        /// <summary>
        /// loads all the stored Boards data from the DB
        /// </summary>
        public void LoadData()
        {
            int maxId = 1;
            IList<BoardDTO> boardDTOs = boardDalController.SelectAllBoards();
            foreach (BoardDTO b in boardDTOs)
            {
                if (b.ID >= maxId)
                {
                    maxId =(int) b.ID + 1;
                }
                Board board = new Board(b);
                board.LoadData();
                Boards.Add(board, new List<string>());
                IList<BoardMemberDTO> boardMembers = boardMemberDalController.SelectAllBoardMembers(b);
                boardMembers.OrderBy((BoardMemberDTO dto) => dto.MemberID);
                foreach (BoardMemberDTO m in boardMembers)
                {
                    Boards[board].Add(m.MemberEmail);
                }
            }
            IdCounter = maxId;
        }

        /// <summary>
        /// returns all the boards where the given user is member of
        /// </summary>
        /// <param name="userEmail">email of the desired user</param>
        /// <returns>a list of all the boards the user created/joined</returns>
        public IList<Board> GetAllBoards(string userEmail)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to get his/her boards, the user should be connected");
            }
            IList<Board> ans = new List<Board>();
            foreach (KeyValuePair<Board, IList<string>> keyValue in Boards)
            {
                if (keyValue.Value.Contains(userEmail))
                {
                    ans.Add(keyValue.Key);
                }
            }
            return ans;
        }
        /// <summary>
        /// returns all the boards where the given user is not member of
        /// </summary>
        /// <param name="userEmail">email of the desired user</param>
        /// <returns>a list of all the boards the user not created/joined</returns>
        public IList<Board> GetAllJoinableBoards(string userEmail)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to get his/her joinable boards, the user should be connected");
            }
            IList<Board> ans = new List<Board>();
            foreach (KeyValuePair<Board, IList<string>> keyValue in Boards)
            {
                if (!keyValue.Value.Contains(userEmail))
                {
                    ans.Add(keyValue.Key);
                }
            }
            return ans;
        }

        /// <summary>
        /// gets all the boards a user can remove(boards he created)
        /// </summary>
        /// <param name="userEmail">user email</param>
        /// <returns>a list of all the boards the user can remove</returns>
        public IList<Board> GetRemovableBoards(string userEmail)
        {
            if (!ConnectedUser.Contains(userEmail))
            {
                throw new Exception("In order to get his/her joinable boards, the user should be connected");
            }
            IList<Board> ans = new List<Board>();
            foreach (KeyValuePair<Board, IList<string>> keyValue in Boards)
            {
                if (keyValue.Key.CreatorEmail==userEmail)
                {
                    ans.Add(keyValue.Key);
                }
            }
            return ans;
        }

        /// <summary>
        /// get all the emails of members of a desired board
        /// </summary>
        /// <param name="userEmail">email of the current user, should be connected and be a board member</param>
        /// <param name="creatorEmail">email of board creator</param>
        /// <param name="boardName">name of the board</param>
        /// <returns>list of all board members</returns>
        public IList<string> GetBoardMembers(string userEmail, string creatorEmail, string boardName)
        {
            return Boards[GetBoard(userEmail, creatorEmail, boardName)];
        }
    }
}

