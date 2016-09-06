using Autofac;
using Ik.Framework.DataAccess.DataMapping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Ik.Framework.DataAccess.EF;
using Ik.Framework.Events;
using Ik.Framework.Logging;

namespace Ik.Framework.DependencyManagement
{
    public class IkEngine : IEngine
    {
        private static ILog logger = LogManager.GetLogger(ObjectEngineContext.LogModelName_ObjectService);
        #region Fields

        private ContainerManager _containerManager;

        private AppDomainTypeFinder _typeFinder = new AppDomainTypeFinder();

        protected AppDomainTypeFinder TypeFinder
        {
            get { return _typeFinder; }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="config">Config</param>
        protected virtual void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            //we create new instance of ContainerBuilder
            //because Build() or Update() method can only be called once on a ContainerBuilder.

            

            builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(_typeFinder).As<ITypeFinder>().SingleInstance();
            builder.Update(container);

            //register dependencies provided by other assemblies
            builder = new ContainerBuilder();
            var drTypes = _typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
            {
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));
            }
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
            {
                logger.Info(string.Format("发现自动对象注册器，类型：{0}", dependencyRegistrar.GetType().FullName));
                dependencyRegistrar.Register(builder, _typeFinder);
            }
            logger.Info(string.Format("成功注册自动对象注册器{0}个", drInstances.Count));
            builder.Update(container);

            builder = new ContainerBuilder();
            //发现自动服务
            var autoDataServiceTypes = _typeFinder.FindClassesOfAttribute<AutoDataServiceAttribute>(false);
            foreach (var item in autoDataServiceTypes)
            {
                var service = item.GetCustomAttribute<AutoDataServiceAttribute>();
                if (service.ServiceType == AutoDataServiceType.Mapping)
                {
                    logger.Info(string.Format("发现ibatis数据服务对象，类型：{0}", item.FullName));
                    var instance = AutoDataAccessFactory.GetDataAccess(item);
                    builder.RegisterInstance(instance).As(item).SingleInstance();
                }
                else
                {
                    logger.Info(string.Format("发现EF数据服务对象，类型：{0}", item.FullName));
                    var instance = AutoEfRepositoryFactory.GetEfRepository(item);
                    var instanceType = typeof(IRepository<>).MakeGenericType(item);
                    builder.RegisterInstance(instance).As(instanceType).SingleInstance();
                }
            }
            logger.Info(string.Format("成功注册数据服务对象{0}个", autoDataServiceTypes.Count()));
            var autoBizServiceTypes = _typeFinder.FindClassesOfAttribute<AutoBizServiceAttribute>(false);
            foreach (var item in autoBizServiceTypes)
            {
                var ifaceName = item.FullName.Replace("." + item.Name, ".I" + item.Name);
                Type ifaceType = Type.GetType(ifaceName + "," + item.Assembly.FullName);
                if (ifaceType != null)
                {
                    logger.Info(string.Format("发现业务服务对象，类型：{0}，接口类型：{1}", item.FullName, ifaceType.FullName));
                    builder.RegisterType(item).As(ifaceType).SingleInstance();
                }
                else
                {
                    logger.Info(string.Format("发现业务服务对象，类型：{0}", item.FullName));
                    builder.RegisterType(item).SingleInstance();
                }
            }
            logger.Info(string.Format("成功注册业务服务对象{0}个", autoBizServiceTypes.Count()));
            if (SubscriptionManager.IsPublisherContextInitialize)
            {
                logger.Info("事件发布上下文已开启，开始注册事件发布接口IEventPublisher");
                //注册本地事件通知服务
                builder.RegisterInstance(SubscriptionManager.Publisher).As(typeof(Ik.Framework.Events.IEventPublisher)).SingleInstance();
            }

            builder.Update(container);
            this._containerManager = CreateContainerManager(container);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize()
        {
            //register dependencies
            RegisterDependencies();
        }

        protected virtual ContainerManager CreateContainerManager(IContainer container)
        {
            return new ContainerManager(container);
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        /// <summary>
        ///  Resolve dependency
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <returns></returns>
        public T[] ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Container manager
        /// </summary>
        public  ContainerManager ContainerManager
        {
            get { return _containerManager; }
        }

        #endregion
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
