using Autofac;
using Autofac.Integration.WebApi;
using Ik.Framework.DependencyManagement;
using System.Web.Http;

namespace Ik.WebFramework.Api.DependencyManagement
{
    public class IkWebApiEngine : IkEngine
    {
        protected override ContainerManager CreateContainerManager(IContainer container)
        {
            var builder = new ContainerBuilder();
            var list = this.TypeFinder.FindClassesOfType<ApiController>();
            foreach (var item in list)
            {
                builder.RegisterType(item);
            }
            builder.Update(container);
            AutofacWebApiDependencyResolver resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            return new ContainerManager(container, resolver.GetRequestLifetimeScope());
        }
    }
}
