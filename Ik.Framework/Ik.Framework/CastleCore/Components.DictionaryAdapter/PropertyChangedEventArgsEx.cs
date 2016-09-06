// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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
	using System.ComponentModel;

	public class PropertyChangedEventArgsEx : PropertyChangedEventArgs
	{
		private readonly object oldValue;
		private readonly object newValue;

		public PropertyChangedEventArgsEx(string propertyName, object oldValue, object newValue)
			: base(propertyName)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public object OldValue
		{
			get { return oldValue; }
		}

		public object NewValue
		{
			get { return newValue; }
		}
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
