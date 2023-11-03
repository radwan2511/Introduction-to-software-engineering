using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    /// <summary>
    /// DTO abstract class
    /// represtents a record at some table in our Kanban system DB
    /// </summary>
    internal abstract class DTO
    {
        /// <summary>
        /// the dal controller which accesses the appropriate table
        /// </summary>
        protected DalController _controller;
        /// <summary>
        /// the titles of primary keys in the relevant table
        /// </summary>
        public readonly string[] _primaryKeys;
        /// <summary>
        /// the assigned values of a table record to each primay key respectively, can be
        /// either a long or a string(thus we use an object array)
        /// </summary>
        public readonly Object[] _primaryVals;

        /// <summary>
        /// constructor of DTO
        /// </summary>
        /// <param name="controller">the dal controller which accesses the appropriate table</param>
        /// <param name="primaryKeys">the titles of primary keys in the relevant table</param>
        /// <param name="primaryVals">the assigned values of a table record to each primay key respectively, can be
        /// either a long or a string(thus we use an object array)</param>
        protected DTO(DalController controller, string[] primaryKeys, Object[] primaryVals)
        {
            _controller = controller;
            _primaryKeys = primaryKeys;
            _primaryVals = primaryVals;
        }
    }
}
