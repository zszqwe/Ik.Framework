// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace IkCastle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;

	/// <summary>
	/// Contract for creating additional Dictionary adapters.
	/// </summary>
	public interface IDictionaryCreate
	{
		T Create<T>();

		object Create(Type type);

		T Create<T>(IDictionary dictionary);

		object Create(Type type, IDictionary dictionary);

		T Create<T>(Action<T> init);

		T Create<T>(IDictionary dictionary, Action<T> init);
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
