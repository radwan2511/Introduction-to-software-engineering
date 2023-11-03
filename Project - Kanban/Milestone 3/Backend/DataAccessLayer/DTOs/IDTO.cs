using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    interface IDTO
    {
        public string[] _primaryKeys { get; }
        public Object[] _primaryVals { get; }
    }
}
