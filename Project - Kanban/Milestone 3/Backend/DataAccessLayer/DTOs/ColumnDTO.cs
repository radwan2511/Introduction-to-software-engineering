using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// class ColumnDTO
    /// represents a record in table Columns in DB(Board ID,Column Name,Tasks Limit)
    /// </summary>
    class ColumnDTO : DTO, IColumnDTO
    {
        /// <summary>
        /// column names(Columns table in DB):
        /// </summary>
        public const string LimitColumnName = "TasksLimit";
        public const string NameColumnName = "Name";
        public const string BoardIDColumnName = "BoardID";
        public const string ColOrdColumnName = "ColumnOrdinal";

        long IColumnDTO.Limit { get => _limit; set { _controller.Update(_primaryKeys, _primaryVals, LimitColumnName, value); } }
        string IColumnDTO.Name { get => _name; set { _name = value; _controller.Update(_primaryKeys, _primaryVals, NameColumnName, value); } }
        long IColumnDTO.ID { get => _id; set => throw new NotImplementedException(); }
        long IColumnDTO.ColOrdinal { get => _colOrdinal; set { _controller.Update(_primaryKeys, _primaryVals, ColOrdColumnName, value); _colOrdinal = value; _primaryVals[1] = _colOrdinal; } }

        /// <summary>
        /// column limit(of tasks) field and its getter & setter
        /// </summary>
        private long _limit;
        public virtual long Limit { get => _limit; set { _limit = value; _controller.Update(_primaryKeys, _primaryVals, LimitColumnName, value); } }
        /// <summary>
        /// column name field and its getter & setter
        /// </summary>
        private string _name;
        public virtual string Name { get => _name; set { _name = value; _controller.Update(_primaryKeys, _primaryVals, NameColumnName, value); } }

        /// <summary>
        /// board id field and its getter
        /// </summary>
        private long _id;
        public virtual long ID { get => _id; }
        /// <summary>
        /// column ordinal field and its getter & setter
        /// </summary>
        private long _colOrdinal;
        public virtual long ColOrdinal { get => _colOrdinal; set { _controller.Update(_primaryKeys, _primaryVals, ColOrdColumnName, value); _colOrdinal = value; _primaryVals[1] = _colOrdinal; } }

        /// <summary>
        /// ColumnDTO constructor
        /// </summary>
        /// <param name="limit">column tasks limit</param>
        /// <param name="name">column name</param>
        /// <param name="id">board id</param>
        public ColumnDTO(long limit, string name, long id, long colOrd) : base(new ColumnDalController(), new string[] { BoardIDColumnName, ColOrdColumnName }, new object[] { id, colOrd })
        {
            _id = id;
            _name = name;
            _limit = limit;
            _colOrdinal = colOrd;
        }
    }
}
