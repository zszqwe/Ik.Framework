using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using IkCaching.Memcached.Results;
using IkCaching.Memcached.Results.Extensions;

namespace IkCaching.Memcached.Protocol.Binary
{
	public class StatsOperation : BinaryOperation, IStatsOperation
	{
		private static readonly IkCaching.ILog log = IkCaching.LogManager.GetLogger(typeof(StatsOperation));

		private string type;
		private Dictionary<string, string> result;

		public StatsOperation(string type)
		{
			this.type = type;
		}

		protected override BinaryRequest Build()
		{
			var request = new BinaryRequest(OpCode.Stat);
			if (!String.IsNullOrEmpty(this.type))
				request.Key = this.type;

			return request;
		}

		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			var response = new BinaryResponse();
			var serverData = new Dictionary<string, string>();
			var retval = false;

			while (response.Read(socket) && response.KeyLength > 0)
			{
				retval = true;

				var data = response.Data;
				var key = BinaryConverter.DecodeKey(data.Array, data.Offset, response.KeyLength);
				var value = BinaryConverter.DecodeKey(data.Array, data.Offset + response.KeyLength, data.Count - response.KeyLength);

				serverData[key] = value;
			}

			this.result = serverData;
			this.StatusCode = response.StatusCode;

			var result = new BinaryOperationResult()
			{
				StatusCode = StatusCode
			};

			result.PassOrFail(retval, "Failed to read response");
			return result;
		}

		Dictionary<string, string> IStatsOperation.Result
		{
			get { return this.result; }
		}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kisk? enyim.com
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion

#region copyright
/*
*.NET基础开发框架
*Copyright (C) 。。。
*地址：git@github.com:gangzaicd/Ik.Framework.git
*作者：到大叔碗里来（大叔）
*QQ：397754531
*eMail：gangzaicd@163.com
*/
#endregion copyright
