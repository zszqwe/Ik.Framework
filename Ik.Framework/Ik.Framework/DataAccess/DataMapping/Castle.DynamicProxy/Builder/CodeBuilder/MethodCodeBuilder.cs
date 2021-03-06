// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace IBatisNet.CastleDynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection.Emit;

	using IBatisNet.CastleDynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for MethodCodeBuilder.
	/// </summary>
	
	public class MethodCodeBuilder : AbstractCodeBuilder
	{
		private Type _baseType;
		private MethodBuilder _methodbuilder;

		public MethodCodeBuilder( Type baseType, MethodBuilder methodbuilder, ILGenerator generator) : 
			base(generator)
		{
			_baseType = baseType;
			_methodbuilder = methodbuilder;
		}

		private MethodBuilder Builder
		{
			get { return _methodbuilder; }
		}

//		public void Nop()
//		{
//			SetNonEmpty();
//
//			if (Builder.ReturnType == typeof(void))
//			{
//				Generator.Emit( OpCodes.Nop );
//			}
//			else
//			{
//				Generator.Emit( LdcOpCodesDictionary.Instance[Builder.ReturnType], 0 );
//			}
//		}
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
