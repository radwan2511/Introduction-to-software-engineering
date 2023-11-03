using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("Tests")]
namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    interface IColumnDTO : IDTO
    {
        public long Limit { get; set; }
        public string Name { get; set; }
        public long ID { get; set; }
        public long ColOrdinal { get; set; }
    }
}

