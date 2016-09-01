using Ik.Framework.Common.Paging;
using Ik.Framework.DataAccess;
using Ik.Framework.DataAccess.DataMapping;
using Ik.Framework.DependencyManagement;
using Ik.ItAdmin.Web.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.ItAdmin.Web.DataAccess
{
    [AutoDataServiceAttribute]
    [DataSource(DataSources.DataSource_ItAdmin)]
    public interface ICfgManagerDao
    {
        [DataAccessOutSqlText(true, OutSqlTextType.Console)]
        IList<CfgDefinitionVersion> GetCfgDefinitionVersionList();

        CfgDefinition GetCfgDefinitionById(Guid id);

        CfgDefinitionVersion GetCfgDefinitionVersionById(Guid id);

        void InsertCfgDefinition(CfgDefinition model);

        void UpdateAllCfgDefinition(CfgDefinition model);

        void InsertCfgDefinitionVersion(CfgDefinitionVersion model);

        void UpdateAllCfgDefinitionVersion(CfgDefinitionVersion model);

        IList<CfgEnvironment> GetCfgEnvironmentList();

        CfgEnvironment GetCfgEnvironmentById(Guid id);

        void InsertCfgEnvironment(CfgEnvironment model);

        void UpdateAllCfgEnvironment(CfgEnvironment model);

        IPagedList<CfgItemVersionEnvironment> GetCfgItemListByPager(CfgItemVersionEnvironmentParam filter);

        CfgItem GetCfgItemById(Guid id);

        CfgItemVersion GetCfgItemVersionById(Guid id);

        IList<CfgItemVersion> GetCfgItemVersionList(Guid itemId);

        CfgItemVersionEnvironment GetCfgItemVersionEnvironmentById(Guid id);

        void InsertCfgItem(CfgItem model);
        void InsertCfgItemVersion(CfgItemVersion model);
        void InsertCfgItemVersionEnvironment(CfgItemVersionEnvironment model);

        void UpdateAllCfgItem(CfgItem model);

        void UpdateAllCfgItemVersion(CfgItemVersion model);

        void UpdateAllCfgItemVersionEnvironment(CfgItemVersionEnvironment model);

        void DeleteAllCfgItem(IList<Guid> idList);

        IPagedList<CfgItemVersionEnvironment> GetCfgDefinitionItemListByPager(CfgItemVersionEnvironmentParam filter);

        void InsertCfgDefinitionItem(Guid id, Guid defVerId);

        void DeleteAllCfgDefinitionItem(IList<Guid> idList);

        void DeleteAllCfgItemVersion(IList<Guid> idList);

        void DeleteAllCfgItemVersionEnvironment(IList<Guid> idList);

        void DeleteCfgDefinition(Guid id);

        void DeleteCfgDefinitionVersion(Guid id);

        IList<CfgItemVersionEnvironment> GetCfgDefinitionVersionItemList(Guid id);

        IPagedList<CfgItemVersionEnvironment> GetCfgDefinitionVersionItemListByPager(CfgItemVersionEnvironmentParam filter);

        IPagedList<CfgItemVersionEnvironment> GetSelectCfgItemListByPager(CfgItemVersionEnvironmentParam filter);
    }
}
