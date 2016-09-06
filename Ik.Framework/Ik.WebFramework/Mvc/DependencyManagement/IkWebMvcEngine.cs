using Autofac;
using Autofac.Integration.Mvc;
using Ik.Framework.DependencyManagement;
using System.Web.Mvc;

namespace Ik.WebFramework.Mvc.DependencyManagement
{
    public class IkWebMvcEngine : IkEngine
    {
        protected override ContainerManager CreateContainerManager(IContainer container)
        {
            var builder = new ContainerBuilder();
            var list = this.TypeFinder.FindClassesOfType<Controller>();
            foreach (var item in list)
            {
                builder.RegisterType(item);
            }
            builder.Update(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return new ContainerManager(container, AutofacDependencyResolver.Current.RequestLifetimeScope);
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
