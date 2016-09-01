using Ik.Framework.Configuration;
using Ik.Framework.DependencyManagement;
using Ik.Framework.Events;
using Ik.Framework.Logging;
using Ik.WebFramework.Api.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Ik.Framework.Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        Ik.Framework.Logging.ILog logger = Ik.Framework.Logging.LogManager.GetLogger();
        protected void Application_Start()
        {
            //日志初始化
            LogManager.Initialize();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            

            //远程配置初始化
            ConfigManager.RemoteConfigService.ServiceException += RemoteConfigService_ServiceException;
            ConfigManager.RemoteConfigService.Start();

            LogManager.EnableRemoteConfig();

            //事件系统初始化
            SubscriptionManager.SubscriberContextInitialize();
            SubscriptionManager.PublisherContextInitialize();
            SubscriptionManager.DistributedEventService.ServiceException += DistributedEventService_ServiceException;
            SubscriptionManager.DistributedEventService.Start();

            //对象容器初始化
            ObjectEngineContext.Initialize(() => new IkWebApiEngine());

            
        }

        void DistributedEventService_ServiceException(object sender, Ik.Framework.ErrorEventArgs e)
        {
            logger.Error("分布式事件服务错误", e.Exception);
        }

        void RemoteConfigService_ServiceException(object sender, Ik.Framework.ErrorEventArgs e)
        {
            logger.Error("远程配置服务错误", e.Exception);
        }

        protected void Application_End()
        {
            SubscriptionManager.DistributedEventService.Stop();
            ConfigManager.RemoteConfigService.Stop();
        }
    }


    public class Order
    {
        public string Name { get; set; }
    }

    [DistributedEvent(Ik.Framework.Events.HandleType.Sync, "Order")]
    public class DistributedConsumer : IConsumer<Order>
    {
        public void HandleEvent(string lable, Order eventMessage)
        {
            throw new NotImplementedException();
        }
    }
}
