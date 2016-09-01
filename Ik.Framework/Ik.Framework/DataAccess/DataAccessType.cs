using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess
{
    public enum DataAccessType
    {

        Insert, Delete, Update, SelectReadOnly, SelectReadWrite
    }

    public enum DataAccessContextType
    {
        Normal,  ReadWrite
    }


}
