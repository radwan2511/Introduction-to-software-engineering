using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// Board Member DTO class
    /// represents a record in the table BoardMembers(Member ID,Board ID,Member Email) which represents all the
    /// relations of a user who created/joined some board
    /// </summary>
    class BoardMemberDTO:DTO
    {
        /// <summary>
        /// column names(BoardMembers table):
        /// </summary>
        public const string MemberEmailColumnName = "UserEmail";
        public const string BoardIDColumnName = "BoardID";
        public const string MemberIDColumnName = "MemberID";
        /// <summary>
        /// board member email field and its getter & setter
        /// </summary>        
        private string _memberEmail;
        public string MemberEmail { get => _memberEmail; private set { _memberEmail = value; _controller.Update(_primaryKeys,_primaryVals, MemberEmailColumnName, value); } }
        /// <summary>
        /// board id field and its getter
        /// </summary>
        private long _id;
        public long ID { get => _id; }
        /// <summary>
        /// board member id field and its getter
        /// </summary>
        private long _memberId;
        public long MemberID { get => _memberId; }


        /// <summary>
        /// BoardMemberDTO constructor
        /// </summary>
        /// <param name="memberEmail">board member email</param>
        /// <param name="id">board id</param>
        /// <param name="memberID">board member id</param>
        public BoardMemberDTO(string memberEmail,long id,long memberID):base(new BoardMemberDalController(),new string[] { BoardIDColumnName,MemberIDColumnName},new object[] { id,memberID})
        {
            _memberId = memberID;
            _id = id;
            _memberEmail = memberEmail;
        }
    }
}
