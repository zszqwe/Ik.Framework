
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using IBatisNet.DataMapper.SessionStore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Ik.Framework.DataAccess.DataMapping
{
    internal class SqlMapLoader
    {
        private ISqlMapper writeMapper = null;
        private ISqlMapper readOnlyMapper = null;
        private XmlDocument sqlMapConfig = null;
        private string writeConnection = null;
        private string readOnlyConnection = null;
        private DomSqlMapBuilder writeBuilder = null;

        public DomSqlMapBuilder WriteBuilder
        {
            get { return writeBuilder; }
            set { writeBuilder = value; }
        }
        private DomSqlMapBuilder readOnlyBuilder = null;

        public DomSqlMapBuilder ReadOnlyBuilder
        {
            get { return readOnlyBuilder; }
            set { readOnlyBuilder = value; }
        }


        public SqlMapLoader(XmlDocument sqlMapConfig, string writeConnection, string readOnlyConnection)
        {
            if (string.IsNullOrEmpty(writeConnection))
            {
                throw new ArgumentNullException("读写库接不能为空。");
            }
            this.sqlMapConfig = sqlMapConfig;
            this.writeConnection = writeConnection;
            this.readOnlyConnection = readOnlyConnection;
            InitSqlMap();
        }

        private void InitSqlMap()
        {
            try
            {
                writeBuilder = new DomSqlMapBuilder();
                writeBuilder.ValidateSqlMapConfig = false;
                writeMapper = writeBuilder.Configure(this.sqlMapConfig);
                writeMapper.SessionStore = new HybridWebThreadSessionStore(writeMapper.Id);
                writeMapper.DataSource.ConnectionString = this.writeConnection;

                if (!string.IsNullOrEmpty(this.readOnlyConnection))
                {
                    readOnlyBuilder = new DomSqlMapBuilder();
                    readOnlyBuilder.ValidateSqlMapConfig = false;
                    readOnlyMapper = readOnlyBuilder.Configure(this.sqlMapConfig);
                    readOnlyMapper.SessionStore = new HybridWebThreadSessionStore(readOnlyMapper.Id);
                    readOnlyMapper.DataSource.ConnectionString = this.readOnlyConnection;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(string.Format("SqlMap对象创建失败，读写库：{0}，只读库：{1}，配置文件：{2}", ex, this.writeConnection, this.readOnlyConnection, this.sqlMapConfig.InnerXml), ex);
            }
        }
        
        public ISqlMapper WriteMapper
        {
            get { return writeMapper; }
            set { writeMapper = value; }
        }
        
        public ISqlMapper ReadOnlyMapper
        {
            get { return readOnlyMapper; }
            set { readOnlyMapper = value; }
        }
    }
}
