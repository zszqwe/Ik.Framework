using Ik.Framework.BufferService;
using Ik.Framework.Configuration;
using Ik.Framework.DataAccess.DataMapping;
using Ik.Framework.Common.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ik.WebFramework.Api
{
    public class RequestToDataBase : BufferProcessorService
    {
        private IDataAccessFactory factory = null;
        private DataTable logTable = null;
        private DataTable logResponseTable = null;
        private string appName = string.Empty;
        private string serverName = string.Empty;
        public RequestToDataBase(IDataAccessFactory factory)
        {
            this.factory = factory;
            logTable = new DataTable();
            logTable.TableName = "log_request_mvc";
            logTable.Columns.Add("request_id", typeof(Guid));
            logTable.Columns.Add("application_name", typeof(string));
            logTable.Columns.Add("server_name", typeof(string));
            logTable.Columns.Add("client_ip", typeof(string));
            logTable.Columns.Add("client_ip2", typeof(string));
            logTable.Columns.Add("user_code", typeof(string));

            logTable.Columns.Add("client_width", typeof(int));
            logTable.Columns.Add("client_height", typeof(int));
            
            logTable.Columns.Add("http_user_agent", typeof(string));
            logTable.Columns.Add("http_method", typeof(string));
            logTable.Columns.Add("http_status", typeof(int));

            logTable.Columns.Add("request_time", typeof(DateTime));
            logTable.Columns.Add("request_elapsed_milliseconds", typeof(int));
            logTable.Columns.Add("request_url", typeof(string));
            logTable.Columns.Add("request_data", typeof(string));
            logTable.Columns.Add("request_headers", typeof(string));
            logTable.Columns.Add("request_cookie", typeof(string));
            logTable.Columns.Add("response_cookie", typeof(string));

            logTable.Columns.Add("create_time", typeof(DateTime));
            logResponseTable = new DataTable();
            logResponseTable.TableName = "log_request_mvc_result";
            logResponseTable.Columns.Add("request_id", typeof(Guid));
            logResponseTable.Columns.Add("result_content", typeof(string));
            logResponseTable.Columns.Add("create_time", typeof(DateTime));
            appName = AppInfo.AppName;
            serverName = AppInfo.MachineName;
        }

        private void SetDbValue(DataRow dr, string col, object value)
        {
            if (value == null)
            {
                dr[col] = DBNull.Value;
            }
            else
            {
                dr[col] = value;
            }
        }

        public static string CheckStringLen(string value, int len)
        {
            if (value == null)
            {
                return "";
            }
            var strLen = GetCharLength(value);
            if (strLen > len)
            {
                return SubCharString(value, len);
            }
            return value;
        }

        private static string SubCharString(string str, int len)
        {
            if (str == null || str.Length == 0) { return str; }
            StringBuilder sb = new StringBuilder();
            int realLen = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if ((int)str[i] > 128)
                {
                    realLen += 2;
                }
                else
                {
                    realLen += 1;
                }
                if (realLen <= len)
                {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取字符串长度。与string.Length不同的是，该方法将中文作 2 个字符计算。
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <returns></returns>
        private static int GetCharLength(string str)
        {
            if (str == null || str.Length == 0) { return 0; }

            int l = str.Length;
            int realLen = l;

            #region 计算长度
            int clen = 0;//当前长度
            while (clen < l)
            {
                //每遇到一个中文，则将实际长度加一。
                if ((int)str[clen] > 128) { realLen++; }
                clen++;
            }
            #endregion

            return realLen;
        }


        public override void BufferProcess(IList<BufferItem> items)
        {
            DataTable requestLogTable = logTable.Clone();
            DataTable templogResponseTable = logResponseTable.Clone();
            foreach (var item in items)
            {
                var requestInfo = (RequestProcessInfo)item;
                var row = requestLogTable.NewRow();
                SetDbValue(row, "request_id", requestInfo.Id);
                SetDbValue(row, "application_name", appName);
                SetDbValue(row, "server_name", serverName);
                SetDbValue(row, "client_ip", requestInfo.ClientIP);
                SetDbValue(row, "client_ip2", requestInfo.ClientIP2);

                if (requestInfo.IdentityInfo != null)
                {
                    SetDbValue(row, "user_code", requestInfo.IdentityInfo.RefUserCode);
                }

                
                SetDbValue(row, "client_width", requestInfo.ClientWidth);
                SetDbValue(row, "client_height", requestInfo.ClientHeight);
                SetDbValue(row, "http_user_agent", CheckStringLen(requestInfo.UserAgent, 500));
                SetDbValue(row, "http_method", requestInfo.HttpMethod);
                SetDbValue(row, "http_status", requestInfo.HttpStatus);

                SetDbValue(row, "request_time", requestInfo.RequestStartTime);
                SetDbValue(row, "request_elapsed_milliseconds", requestInfo.ElapsedMilliseconds);

                SetDbValue(row, "request_url", CheckStringLen(requestInfo.RequestUrl, 500));

                SetDbValue(row, "request_data", CheckStringLen(requestInfo.RequestArguments == null ? null : requestInfo.RequestArguments.ToJsonString(), 4000));
                SetDbValue(row, "request_headers", CheckStringLen(requestInfo.Headers, 500));
                SetDbValue(row, "request_cookie", CheckStringLen(requestInfo.RequestCookie, 500));
                SetDbValue(row, "response_cookie", CheckStringLen(requestInfo.ResponseCookie, 500));


                row["create_time"] = DateTime.Now;


                requestLogTable.Rows.Add(row);

                if (!string.IsNullOrEmpty(requestInfo.ResultContent))
                {
                    var responseRow = templogResponseTable.NewRow();
                    SetDbValue(responseRow, "request_id", requestInfo.Id);
                    SetDbValue(responseRow, "result_content", CheckStringLen(requestInfo.ResultContent, 8000));
                    responseRow["create_time"] = DateTime.Now;
                    templogResponseTable.Rows.Add(responseRow);
                }
            }
            if (requestLogTable.Rows.Count > 0)
            {
                using (var trans = factory.CreateTransactionScope())
                {
                    factory.SqlBulkCopyInsert(requestLogTable);
                    if (templogResponseTable.Rows.Count > 0)
                    {
                        factory.SqlBulkCopyInsert(templogResponseTable);
                    }
                    trans.Complete();
                }
            }
        }

        public override bool FilterBuffer(BufferItem item)
        {
            return item.TypeName == "RequestProcessInfo";
        }
    }
}
