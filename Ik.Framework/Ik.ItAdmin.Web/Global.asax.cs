using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using Ik.Framework.Configuration;
using Ik.Framework.Logging;
using Ik.Framework.DependencyManagement;
using Autofac;
using Autofac.Integration.Mvc;
using Ik.ItAdmin.Web.Services;
using Ik.Framework;
using System.IO;
using Ik.Framework.DataAccess.DataMapping;
using Ik.WebFramework.Mvc.DependencyManagement;
using Ik.WebFramework.Mvc;
using Ik.Framework.Events;
using Ik.Service.Auth;
using Ik.ItAdmin.Web.Infrastructure;
using Ik.Framework.BufferService;
using Ik.WebFramework.Api;


namespace Ik.ItAdmin.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        Ik.Framework.Logging.ILog logger = Ik.Framework.Logging.LogManager.GetLogger();
        protected void Application_Start()
        {
            var filters = GlobalFilters.Filters;
            filters.Add(new Ik.WebFramework.Mvc.RequestProcessHandleAttribute(true, true));
            filters.Add(new System.Web.Mvc.ValidateInputAttribute(false));
            var routes = RouteTable.Routes;
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            ConfigManager.RemoteConfigService.ServiceException += RemoteConfigService_ServiceException;
            ConfigManager.RemoteConfigService.Start();

            LogManager.Initialize();

            BufferServiceManager.AddBufferProcessorService(new List<BufferProcessorService> { new RequestToDataBase(OperationSupportDataAccessFactory.Instance) });

            BufferServiceManager.DispatcherService.ServiceException += DispatcherService_ServiceException;
            BufferServiceManager.DispatcherService.Start();

            
            SubscriptionManager.SubscriberContextInitialize();
            SubscriptionManager.PublisherContextInitialize();

            SubscriptionManager.DistributedEventService.ServiceException += DistributedEventService_ServiceException;

            SubscriptionManager.DistributedEventService.Start();
            
            IkAuthorizationContext.Initialize(() => new ItAdminIkAuthorization());

            ObjectEngineContext.Initialize(() => new IkWebMvcEngine());

        }

        void RemoteConfigService_ServiceException(object sender, Ik.Framework.ErrorEventArgs e)
        {
            logger.Error("远程配置服务错误",e.Exception);
        }

        void DispatcherService_ServiceException(object sender, Ik.Framework.ErrorEventArgs e)
        {
            logger.Error("数据缓冲服务初始化异常", e.Exception);
        }

        void DistributedEventService_ServiceException(object sender, Ik.Framework.ErrorEventArgs e)
        {
            logger.Error("分布式事件服务错误", e.Exception);
        }

        protected void Application_End()
        {
            SubscriptionManager.DistributedEventService.Stop();

            BufferServiceManager.DispatcherService.Stop();

            ConfigManager.RemoteConfigService.Stop();
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
