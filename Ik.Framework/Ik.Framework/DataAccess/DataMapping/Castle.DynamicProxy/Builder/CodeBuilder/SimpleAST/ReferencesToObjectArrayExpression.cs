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

namespace IBatisNet.CastleDynamicProxy.Builder.CodeBuilder.SimpleAST
{
	using System;
	using System.Reflection.Emit;
	using IBatisNet.CastleDynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for ReferencesToObjectArrayExpression.
	/// </summary>
	
	public class ReferencesToObjectArrayExpression : Expression
	{
		private TypeReference[] _args;

		public ReferencesToObjectArrayExpression( params TypeReference[] args )
		{
			_args = args;
		}

		public override void Emit(IEasyMember member, ILGenerator gen)
		{
			LocalBuilder local = gen.DeclareLocal( typeof(object[]) );
			gen.Emit(OpCodes.Ldc_I4, _args.Length);
			gen.Emit(OpCodes.Newarr, typeof(object));
			gen.Emit(OpCodes.Stloc, local);
			
			for(int i=0; i < _args.Length; i++)
			{
				gen.Emit(OpCodes.Ldloc, local);
				gen.Emit(OpCodes.Ldc_I4, i);

				TypeReference reference = _args[i];

				ArgumentsUtil.EmitLoadOwnerAndReference(reference, gen);

				if (reference.Type.IsByRef)
				{
					throw new NotSupportedException();
				}

				if (reference.Type.IsValueType)
				{
					gen.Emit(OpCodes.Box, reference.Type.UnderlyingSystemType);
				}
				
				gen.Emit(OpCodes.Stelem_Ref);
			}

			gen.Emit(OpCodes.Ldloc, local);
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
