/*
 *  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */
﻿using System;

namespace IkZooKeeperNet
{
    using Common.Logging;

    public class SafeThreadStart
    {
        private readonly Action action;
        private static readonly ILog LOG = LogManager.GetLogger(typeof(SafeThreadStart));

        public SafeThreadStart(Action action)
        {
            this.action = action;
        }

        public void Run()
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                LOG.Error("Unhandled exception in background thread", e);
            }            
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
