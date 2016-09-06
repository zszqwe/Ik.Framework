
using System;
namespace Ik.Framework.Events
{
    /// <summary>
    /// A container for entities that are updated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuthUpdatedEvent:IEventTrack
    {
        private Guid eventId = Guid.NewGuid();
        public Guid EventId
        {
            get 
            {
                return eventId;
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
