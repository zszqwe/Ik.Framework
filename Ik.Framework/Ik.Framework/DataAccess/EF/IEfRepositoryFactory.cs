using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.DataAccess.EF
{
    public interface IEfRepositoryFactory
    {
        IRepository<T> GetEfRepository<T>() where T : BaseEntity;

        object GetEfRepository(Type entityType);

        DataAccessContextScope CreateNormalContextScope();
        DataAccessContextScope CreateReadWriteContextScope();

        void BeginNormalContext();

        void BeginReadWriteContex();

        void EndContext();
    }
}
