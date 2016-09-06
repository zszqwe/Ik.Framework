using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using System.Web;
using IkCaching.Configuration;
using System.Configuration;
using System.IO;

namespace IkCaching.Memcached
{
	/// <summary>
	/// Factory for the vbucket based locator.
	/// </summary>
	/// <remarks>You need to use this in the configuration file because this is the only way pass parameters to the VBucketNodeLocator.
	/// 
	///		<locator factory="IkCaching.Memcached.VBucketNodeLocatorFactory" configFile="vbucket.json" />
	/// 
	/// </remarks>
	public class VBucketNodeLocatorFactory : IProviderFactory<IMemcachedNodeLocator>
	{
		private string hashAlgo;
		private VBucket[] buckets;

		void IProvider.Initialize(Dictionary<string, string> parameters)
		{
			ConfigurationHelper.TryGetAndRemove(parameters, "hashAlgorithm", out this.hashAlgo, true);

			string json;
			ConfigurationHelper.TryGetAndRemove(parameters, String.Empty, out json, true);
			ConfigurationHelper.CheckForUnknownAttributes(parameters);

			var tmp = new JavaScriptSerializer().Deserialize<int[][]>(json);

			this.buckets = tmp.Select(entry => new VBucket(entry[0], entry.Skip(1).ToArray())).ToArray();
		}

		IMemcachedNodeLocator IProviderFactory<IMemcachedNodeLocator>.Create()
		{
			return new VBucketNodeLocator(this.hashAlgo, this.buckets);
		}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kiskó, enyim.com
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
