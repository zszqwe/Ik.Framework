﻿using System;
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
	/// Implements a vbucket based node locator.
	/// </summary>
	public class VBucketNodeLocator : IMemcachedNodeLocator
	{
		private VBucket[] buckets;
		private int mask;
		private Func<HashAlgorithm> factory;

		private VBucketNodeLocator()
		{
			throw new InvalidOperationException("You must use the VBucketNodeLocatorFactory in the configuration file to use this locator!");
		}

		public VBucketNodeLocator(string hashAlgorithm, VBucket[] buckets)
		{
			var log = Math.Log(buckets.Length, 2);
			if (log != (int)log)
				throw new ArgumentException("bucket count must be a power of 2!");

			this.buckets = buckets;
			this.mask = buckets.Length - 1;

			if (!hashFactory.TryGetValue(hashAlgorithm, out this.factory))
				throw new ArgumentException("Unknown hash algorithm: " + hashAlgorithm, "hashAlgorithm");
		}

		//[ThreadStatic]
		//private static HashAlgorithm currentAlgo;

		//private HashAlgorithm GetAlgo()
		//{
		//    // we cache the HashAlgorithm instance per thread
		//    // they are reinitialized before every ComputeHash but we avoid creating then GCing them every time we need 
		//    // to find something (which will happen a lot)
		//    // we cannot use ThreadStatic for asp.net (requests can change threads) so the hasher will be recreated for every request
		//    // (we could use an object pool but talk about overkill)
		//    var ctx = HttpContext.Current;
		//    if (ctx == null)
		//        return currentAlgo ?? (currentAlgo = this.factory());

		//    var algo = ctx.Items["**VBucket.CurrentAlgo"] as HashAlgorithm;
		//    if (algo == null)
		//        ctx.Items["**VBucket.CurrentAlgo"] = algo = this.factory();

		//    return algo;
		//}

		#region [ IMemcachedNodeLocator        ]

		private IMemcachedNode[] nodes;

		void IMemcachedNodeLocator.Initialize(IList<IMemcachedNode> nodes)
		{
			// we do not care about dead nodes
			if (this.nodes != null) return;

			this.nodes = nodes.ToArray();
		}

		IMemcachedNode IMemcachedNodeLocator.Locate(string key)
		{
			var bucket = this.GetVBucket(key);

			return this.nodes[bucket.Master];
		}

		public int GetIndex(string key)
		{
			var ha = this.factory();

			//little shortcut for some hashes; we skip the uint -> byte[] -> uint conversion
			var iuha = ha as IUIntHashAlgorithm;
			var keyBytes = Encoding.UTF8.GetBytes(key);

			uint keyHash = (iuha == null)
							? BitConverter.ToUInt32(ha.ComputeHash(keyBytes), 0)
							: iuha.ComputeHash(keyBytes);

			return (int)(keyHash & this.mask);
		}

		public VBucket GetVBucket(string key)
		{
			int index = GetIndex(key);

			return this.buckets[index];
		}

		IEnumerable<IMemcachedNode> IMemcachedNodeLocator.GetWorkingNodes()
		{
			var nodes = this.nodes;
			var retval = new IMemcachedNode[nodes.Length];

			Array.Copy(nodes, retval, retval.Length);

			return retval;
		}

		#endregion
		#region [ hashFactory                  ]

		private static readonly Dictionary<string, Func<HashAlgorithm>> hashFactory = new Dictionary<string, Func<HashAlgorithm>>(StringComparer.OrdinalIgnoreCase)
		        {
		            { String.Empty, () => new HashkitOneAtATime() },
		            { "default", () => new HashkitOneAtATime() },
		            { "crc", () => new HashkitCrc32() },
		            { "fnv1_32", () => new IkCaching.FNV1() },
		            { "fnv1_64", () => new IkCaching.FNV1a() },
		            { "fnv1a_32", () => new IkCaching.FNV64() },
		            { "fnv1a_64", () => new IkCaching.FNV64a() },
		            { "murmur", () => new HashkitMurmur() }
		        };

		#endregion
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
