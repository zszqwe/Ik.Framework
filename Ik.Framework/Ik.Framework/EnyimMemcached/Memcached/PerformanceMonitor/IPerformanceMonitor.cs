using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace IkCaching.Memcached
{
	public interface IPerformanceMonitor : IDisposable
	{
		void Get(int amount, bool success);
		void Store(StoreMode mode, int amount, bool success);
		void Delete(int amount, bool success);
		void Mutate(MutationMode mode, int amount, bool success);
		void Concatenate(ConcatenationMode mode, int amount, bool success);
	}
}

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
