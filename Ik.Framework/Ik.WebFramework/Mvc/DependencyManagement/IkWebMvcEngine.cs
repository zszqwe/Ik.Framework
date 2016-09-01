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
