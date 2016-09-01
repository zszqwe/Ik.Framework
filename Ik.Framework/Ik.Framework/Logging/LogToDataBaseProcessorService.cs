using Ik.Framework.BufferService;
using Ik.Framework.DataAccess.DataMapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.Framework.Logging
{
    public class LogToDataBase:BufferProcessorService
    {
        private IDataAccessFactory factory = null;
        private DataTable logTable =  new DataTable();
        private DataTable logContentTable =  new DataTable();

        public LogToDataBase(IDataAccessFactory factory)
        {
            this.factory = factory;
            logTable.TableName = "log_common";
            logTable.Columns.Add("log_id", typeof(Guid));
            logTable.Columns.Add("app_name", typeof(string));
            logTable.Columns.Add("model_name", typeof(string));
            logTable.Columns.Add("business_name", typeof(string));
            logTable.Columns.Add("lable", typeof(string));
            logTable.Columns.Add("level", typeof(int));
            logTable.Columns.Add("code", typeof(int));
            logTable.Columns.Add("message", typeof(string));
            logTable.Columns.Add("log_time", typeof(DateTime));
            logTable.Columns.Add("create_time", typeof(DateTime));
            logTable.Columns.Add("server_name", typeof(string));
            logContentTable.TableName = "log_common_content";
            logContentTable.Columns.Add("log_id", typeof(Guid));
            logContentTable.Columns.Add("log_content", typeof(string));
            logContentTable.Columns.Add("create_time", typeof(DateTime));
        }

        private  void SetDbValue(DataRow dr, string col, object value)
        {
            if (value == null)
            {
                dr[col] =  DBNull.Value;
            }
            else
            {
                dr[col] = value;
            }
        }
        public override void BufferProcess(IList<BufferItem> items)
        {
            DataTable templogTable = logTable.Clone();
            DataTable templogContentTable = logContentTable.Clone();
            foreach (var item in items)
            {
                var log = (LogEntity)item;
                var row = templogTable.NewRow();
                SetDbValue(row, "log_id", log.Id);
                SetDbValue(row, "app_name", log.AppName??"");
                SetDbValue(row, "model_name", log.ModelName ?? "");
                SetDbValue(row, "business_name", log.BusinessName ?? "");
                SetDbValue(row, "lable", log.Lable ?? "");
                SetDbValue(row, "server_name", log.ServerName ?? "");
                SetDbValue(row, "level", log.Level);
                SetDbValue(row, "code", log.Code);
                SetDbValue(row, "message", log.Message ?? "");
                SetDbValue(row, "log_time", log.CreateTime);
                SetDbValue(row, "create_time", DateTime.Now);
                templogTable.Rows.Add(row);

                if (!string.IsNullOrEmpty(log.Content))
                {
                    var contentRow = templogContentTable.NewRow();
                    SetDbValue(contentRow, "log_id", log.Id);
                    SetDbValue(contentRow, "log_content", log.Content ?? "");
                    SetDbValue(contentRow, "create_time", DateTime.Now);
                    templogContentTable.Rows.Add(contentRow);
                }
            }
            if (templogTable.Rows.Count > 0)
            {
                using (var trans = factory.CreateTransactionScope())
                {
                    factory.SqlBulkCopyInsert(templogTable);
                    if (templogContentTable.Rows.Count > 0)
                    {
                        factory.SqlBulkCopyInsert(templogContentTable);
                    }
                    trans.Complete();
                }
            }
        }

        public override bool FilterBuffer(BufferItem item)
        {
            return item.TypeName == "LogEntity";
        }
    }
}
